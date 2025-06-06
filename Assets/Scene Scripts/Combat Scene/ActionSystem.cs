using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Splines.Examples;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ActionSystem : MonoBehaviour
{
    public static ActionSystem Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only one allowed
            return;
        }

        Instance = this;
    }

    [System.Serializable]
    public struct Action
    {
        public Targetable sender;
        public CardInformation card;
        public GameObject target;
        public int manaCost; // NEW FIELD

        public Action(Targetable sender, CardInformation card, GameObject target, int manaCost)
        {
            this.sender = sender;
            this.card = card;
            this.target = target;
            this.manaCost = manaCost;
        }
    }

    [SerializeField] private List<Action> actions = new List<Action>();

    public void AddCard(Targetable sender, CardInformation card, GameObject target, int cost)
    {
        actions.Add(new (sender, card, target, cost));
    }

    public void RemoveCard(CardInformation card)
    {
        int index = actions.FindIndex(a => a.card == card);
        if (index != -1)
        {
            actions.RemoveAt(index);
        }
    }

    public void EndTurn()
    {
        StartCoroutine(EndTurnCoroutine());
    }

    private IEnumerator EndTurnCoroutine()
    {
        CharacterGenerator.Instance.DisablePlayerRaycasts();
        CharacterManager.Instance.DisplayCardView();
        DrawLine.Instance.RemoveAllLines();
        EnemyActionsManager.Instance.SetEnemyAction();
        SortActions();
        yield return DoActions();
        DoStatusEffects();
        yield return DiscardAndDrawAllDecks();
        actions.Clear();
        EndTurnRestoreMana();
        CheckForGroupDefeat();
        CharacterManager.Instance.SelectFirstCharacter();
        CharacterGenerator.Instance.EnablePlayerRaycasts();
    }

    //sorts all action by sender's speed stat
    private void SortActions()
    {
        actions.Sort((a, b) =>
        {
            // Handle null cases gracefully
            if (a.card == null || b.card == null) return 0;

            var aCharSPD = a.card.GetComponentInParent<CharacterInfo>().stats.totalSPD;
            var bCharSPD = b.card.GetComponentInParent<CharacterInfo>().stats.totalSPD;

            if (aCharSPD == null || bCharSPD == null) return 0;

            return bCharSPD.CompareTo(aCharSPD); // Descending
        });
    }

    //does all action from fastest to slowest sender
    private IEnumerator DoActions()
    {
        TargetingSystem.Instance.darkPanel.SetActive(true);

        foreach (Action action in actions)
        {
            CharacterInfo senderInfo = action.sender.GetComponent<CharacterInfo>();
            CharacterInfo targetInfo = action.target.GetComponent<CharacterInfo>();
            CardInformation targetCard = targetInfo == null ? action.target.GetComponent<CardInformation>() : null;

            int initialHP = targetInfo != null ? targetInfo.currentHP : -100;
            string audioSFX = "";

            CharacterDeck senderDeck = senderInfo.GetComponent<CharacterDeck>();

            if (senderInfo == null || (targetInfo == null && targetCard == null))
            {
                string senderName = action.sender != null ? action.sender.name : "NULL sender";
                string targetName = action.target != null ? action.target.name : "NULL target";

                Debug.LogWarning($"CharacterInfo missing! Sender: {senderName}, Target: {targetName}");

                continue;
            }

            if (senderInfo == null || senderInfo.currentHP <= 0)
            {
                Debug.Log($"{senderInfo.name} is dead or null");
                continue;
            }

            if (targetInfo != null && targetInfo.currentHP <= 0)
            {
                Debug.Log($"{targetInfo.name} is dead");
                continue;
            }

            if (TargetingSystem.Instance.allies.members.Contains(action.sender))
            {
                yield return new WaitForSeconds(0.1f);
                CharacterManager.Instance.DisplayCardView(senderInfo);
                yield return new WaitForSeconds(0.1f);
            }

            ActionPhaseAnimation.Instance.ActionAnimationStart(action.sender, action.card, action.target);
            yield return senderDeck.StartPlayCard(action.card);

            List<GameObject> targetList = new List<GameObject>();

            if (action.card.card.target == Target.AllEnemies || action.card.card.target == Target.AllAllies)
            {
                targetList = TargetSelector.Instance.GetTargets(action.card, action.sender);

                foreach (GameObject t in targetList)
                {
                    foreach (ICardEffect effect in action.card.card.effects)
                    {
                        effect.Execute(action.sender, action.card, t, action.card.card.mana);
                    }

                    if (t.TryGetComponent<CharacterInfo>(out var tInfo) && tInfo.currentHP <= 0)
                    {
                        // gives xp
                        if (TargetingSystem.Instance.allies.members.Contains(senderInfo.GetComponent<Targetable>()))
                        {
                            int levelDiff = Mathf.Max(tInfo.characterData.basicInfo.level - senderInfo.characterData.basicInfo.level, 1);
                            int XPGain = CalculateXP(senderInfo.characterData.basicInfo.level, levelDiff);

                            float goldMultiplier = Mathf.Max(levelDiff * 0.75f, 1.25f);
                            int goldFound = Mathf.RoundToInt(Random.Range(50, 101) * goldMultiplier);

                            CombatSystem.Instance.AddGoldFound(goldFound);
                            foreach (Targetable ally in TargetingSystem.Instance.allies.members)
                            {
                                CharacterData allyData = ally.GetComponent<CharacterInfo>().characterData;
                                if (allyData == senderInfo.characterData)
                                {
                                    CombatSystem.Instance.AddXP(allyData, XPGain);
                                }
                                else
                                {
                                    CombatSystem.Instance.AddXP(allyData, XPGain / 2);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (ICardEffect effect in action.card.card.effects)
                {
                    effect.Execute(action.sender, action.card, action.target, action.manaCost);
                }

                if (action.target.TryGetComponent<CharacterInfo>(out var tInfo) && tInfo.currentHP <= 0)
                {
                    // gives xp
                    if (TargetingSystem.Instance.allies.members.Contains(senderInfo.GetComponent<Targetable>()))
                    {
                        int levelDiff = Mathf.Max(tInfo.characterData.basicInfo.level - senderInfo.characterData.basicInfo.level, 1);
                        int XPGain = CalculateXP(senderInfo.characterData.basicInfo.level, levelDiff);

                        float goldMultiplier = Mathf.Max(levelDiff * 0.75f, 1.25f);
                        int goldFound = Mathf.RoundToInt(Random.Range(50, 101) * goldMultiplier);

                        CombatSystem.Instance.AddGoldFound(goldFound);
                        foreach (Targetable ally in TargetingSystem.Instance.allies.members)
                        {
                            CharacterData allyData = ally.GetComponent<CharacterInfo>().characterData;
                            if (allyData == senderInfo.characterData)
                            {
                                CombatSystem.Instance.AddXP(allyData, XPGain);
                            }
                            else
                            {
                                CombatSystem.Instance.AddXP(allyData, XPGain / 2);
                            }
                        }
                    }
                }
            }

            if (action.card.card.target != Target.AllEnemies && targetInfo != null)
            {
                int currentHP = targetInfo.currentHP;

                if (currentHP <= 0 && initialHP > 0)
                    audioSFX = "kill";
                else if (currentHP == initialHP)
                    audioSFX = "miss";
                else
                    audioSFX = "hit";
            }
            else
            {
                audioSFX = "hit"; // fallback for AoE
            }

            yield return ActionPhaseAnimation.Instance.ActionAnimationPerform(action.sender, action.card, action.target, audioSFX);

            if (senderInfo.currentHP <= 0)
            {
                senderInfo.currentHP = 0;
                senderInfo.UpdateResourcesView();
                senderInfo.GetComponentInChildren<Animator>().SetTrigger("doDeath");
            }

            if (targetInfo != null && targetInfo.currentHP <= 0)
            {
                targetInfo.currentHP = 0;
                targetInfo.UpdateResourcesView();
            }

            yield return senderDeck.EndPlayCard(action.card);
            ActionPhaseAnimation.Instance.ActionAnimationEnd(action.sender, action.card, action.target);

            yield return new WaitForSeconds(0.1f);
        }

        TargetingSystem.Instance.darkPanel.SetActive(false);
    }

    //apply all status effects
    private void DoStatusEffects()
    {
        List<Targetable> characters = new List<Targetable>();
        characters.AddRange(TargetingSystem.Instance.allies.members);
        characters.AddRange(TargetingSystem.Instance.enemies.members);

        List<CharacterInfo> infoList = characters.Select(c => c.GetComponent<CharacterInfo>()).ToList();

        foreach (CharacterInfo character in infoList)
        {
            int charHP = character.currentHP;
            character.OnTurnStart();

            if (character.currentHP <= 0 && charHP > 0)
            {
                IStatusEffect burnEffect = character.activeEffects.FirstOrDefault(s => s is BurnStatusEffect && ((BurnStatusEffect)s).Sender != null);
                Targetable targetable = character.GetComponent<Targetable>();

                // Only reward XP if the dead character was an enemy and was burned
                if (burnEffect != null && TargetingSystem.Instance.enemies.members.Contains(targetable))
                {
                    CharacterInfo senderInfo = ((BurnStatusEffect)burnEffect).Sender;
                    CharacterInfo targetInfo = character;

                    int levelDiff = Mathf.Max(targetInfo.characterData.basicInfo.level - senderInfo.characterData.basicInfo.level, 1);
                    int XPGain = CalculateXP(senderInfo.characterData.basicInfo.level, levelDiff);

                    float goldMultiplier = Mathf.Max(levelDiff * 0.75f, 1.25f);
                    int goldFound = Mathf.RoundToInt(Random.Range(50, 101) * goldMultiplier);

                    CombatSystem.Instance.AddGoldFound(goldFound);
                    foreach (Targetable ally in TargetingSystem.Instance.allies.members)
                    {
                        CharacterData allyData = ally.GetComponent<CharacterInfo>().characterData;
                        if (allyData == senderInfo.characterData)
                        {
                            CombatSystem.Instance.AddXP(allyData, XPGain);
                        }
                        else
                        {
                            CombatSystem.Instance.AddXP(allyData, XPGain / 2);
                        }
                    }
                }
            }
        }

        foreach (CharacterInfo character in infoList)
        {
            character.OnTurnEnd();
        }

        foreach (CharacterInfo character in infoList)
        {
            if (character.currentHP <= 0)
            {
                character.currentHP = 0;
                character.UpdateResourcesView();

                Animator animator = character.GetComponentInChildren<Animator>();
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                if (!stateInfo.IsName("Death") && !stateInfo.IsName("death"))
                {
                    animator.SetTrigger("doDeath");
                }
            }
        }
    }

    //check if either group is dead
    private void CheckForGroupDefeat()
    {
        bool allAlliesDead = true;
        bool allEnemiesDead = true;

        foreach (Targetable ally in TargetingSystem.Instance.allies.members)
        {
            CharacterInfo info = ally.GetComponent<CharacterInfo>();
            if (info.currentHP > 0)
            {
                allAlliesDead = false;
                break;
            }
        }

        foreach (Targetable enemy in TargetingSystem.Instance.enemies.members)
        {
            CharacterInfo info = enemy.GetComponent<CharacterInfo>();
            if (info.currentHP > 0)
            {
                allEnemiesDead = false;
                break;
            }
        }

        if (allAlliesDead == true)
        {
            CombatSystem.Instance.CombatCompleteLose(allAlliesDead);
        }
        else if (allEnemiesDead == true)
        {
            CombatSystem.Instance.GenerateNewEnemyGroup();
        }
    }

    //do instant action cards with no target (e.g. draw cards)
    public void TriggerAction(Targetable sender, CardInformation card)
    {
        CharacterGenerator.Instance.DisablePlayerRaycasts();

        foreach (ICardEffect effect in card.card.effects)
        {
            effect.Execute(sender, card, null, card.card.mana);
        }

        CharacterGenerator.Instance.EnablePlayerRaycasts();
    }

    //do instant action cards with target (e.g. buff cards)
    public IEnumerator TriggerAction(Targetable sender, CardInformation card, GameObject target)
    {
        CharacterGenerator.Instance.DisablePlayerRaycasts();
        CharacterManager.Instance.DisplayCardView();

        CharacterDeck senderDeck = sender.GetComponent<CharacterDeck>();

        if (target.GetComponent<CardInformation>() == null)
        {
            TargetingSystem.Instance.darkPanel.SetActive(true);
            ActionPhaseAnimation.Instance.ActionAnimationStart(sender, card, target);
        }

        yield return StartCoroutine(senderDeck.StartPlayCard(card));

        List<GameObject> targetList = new List<GameObject>();
                
        if (card.card.target == Target.AllEnemies || card.card.target == Target.AllAllies)
        {
            targetList = TargetSelector.Instance.GetTargets(card, sender);

            foreach (GameObject t in targetList)
            {
                foreach (ICardEffect effect in card.card.effects)
                {
                    if (effect is DrawCardEffect)
                    {
                        effect.Execute(sender, card, null, card.card.mana);
                    }
                    else
                    {
                        effect.Execute(sender, card, t, card.card.mana);
                    }
                }

                if (t.TryGetComponent<CardInformation>(out var cardInfo))
                {
                    cardInfo.UpdateCard();
                }

                if (t.TryGetComponent<CharacterInfo>(out var tInfo) && tInfo.currentHP <= 0)
                {
                    // gives xp
                    if (TargetingSystem.Instance.allies.members.Contains(sender))
                    {
                        CharacterInfo senderInfo = sender.GetComponent<CharacterInfo>();

                        int levelDiff = Mathf.Max(tInfo.characterData.basicInfo.level - senderInfo.characterData.basicInfo.level, 1);
                        int XPGain = CalculateXP(senderInfo.characterData.basicInfo.level, levelDiff);

                        float goldMultiplier = Mathf.Max(levelDiff * 0.75f, 1.25f);
                        int goldFound = Mathf.RoundToInt(Random.Range(50, 101) * goldMultiplier);

                        CombatSystem.Instance.AddGoldFound(goldFound);
                        foreach (Targetable ally in TargetingSystem.Instance.allies.members)
                        {
                            CharacterData allyData = ally.GetComponent<CharacterInfo>().characterData;
                            if (allyData == senderInfo.characterData)
                            {
                                CombatSystem.Instance.AddXP(allyData, XPGain);
                            }
                            else
                            {
                                CombatSystem.Instance.AddXP(allyData, XPGain / 2);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            foreach (ICardEffect effect in card.card.effects)
            {
                if (effect is DrawCardEffect)
                {
                    effect.Execute(sender, card, null, card.card.mana);
                }
                else
                {
                    effect.Execute(sender, card, target, card.card.mana);
                }
            }

            if (target.TryGetComponent<CardInformation>(out var cardInfo))
            {
                cardInfo.UpdateCard();
            }

            if (target.TryGetComponent<CharacterInfo>(out var tInfo) && tInfo.currentHP <= 0)
            {
                // gives xp
                if (TargetingSystem.Instance.allies.members.Contains(sender))
                {
                    CharacterInfo senderInfo = sender.GetComponent<CharacterInfo>();

                    int levelDiff = Mathf.Max(tInfo.characterData.basicInfo.level - senderInfo.characterData.basicInfo.level, 1);
                    int XPGain = CalculateXP(senderInfo.characterData.basicInfo.level, levelDiff);

                    float goldMultiplier = Mathf.Max(levelDiff * 0.75f, 1.25f);
                    int goldFound = Mathf.RoundToInt(Random.Range(50, 101) * goldMultiplier);

                    CombatSystem.Instance.AddGoldFound(goldFound);
                    foreach (Targetable ally in TargetingSystem.Instance.allies.members)
                    {
                        CharacterData allyData = ally.GetComponent<CharacterInfo>().characterData;
                        if (allyData == senderInfo.characterData)
                        {
                            CombatSystem.Instance.AddXP(allyData, XPGain);
                        }
                        else
                        {
                            CombatSystem.Instance.AddXP(allyData, XPGain / 2);
                        }
                    }
                }
            }
        }

        if (target.GetComponent<CardInformation>() == null)
        {
            CharacterInfo targetInfo = target.GetComponent<CharacterInfo>();

            yield return ActionPhaseAnimation.Instance.ActionAnimationPerform(sender, card, target, "hit");

            if (targetInfo.currentHP <= 0)
            {
                targetInfo.currentHP = 0;
            }

            targetInfo.UpdateResourcesView();

            if (sender.GetComponent<CharacterInfo>().currentHP <= 0)
            {
                sender.GetComponent<CharacterInfo>().currentHP = 0;
                sender.GetComponent<CharacterInfo>().UpdateResourcesView();
                sender.GetComponentInChildren<Animator>().SetTrigger("doDeath");
            }
        }

        yield return new WaitForSeconds(0.5f);

        yield return senderDeck.EndPlayCard(card);

        if (target.GetComponent<CardInformation>() == null)
        {
            ActionPhaseAnimation.Instance.ActionAnimationEnd(sender, card, target);
            TargetingSystem.Instance.darkPanel.SetActive(false);
        }

        yield return senderDeck.EndPlaySortHand();

        yield return senderDeck.UpdateSelectedCardPos();

        if (sender.GetComponent<CharacterInfo>().currentHP <= 0)
        {
            CharacterManager.Instance.SelectFirstCharacter();
        }
        CharacterGenerator.Instance.EnablePlayerRaycasts();
    }

    //discards and draws all living units' decks
    private IEnumerator DiscardAndDrawAllDecks()
    {
        List<Targetable> characters = new List<Targetable>();
        characters.AddRange(TargetingSystem.Instance.allies.members);
        characters.AddRange(TargetingSystem.Instance.enemies.members);

        List<CharacterDeck> deckList = characters
            .Select(c => c.GetComponent<CharacterDeck>())
            .ToList();

        int completed = 0;

        foreach (CharacterDeck deck in deckList)
        {
            StartCoroutine(DeckCoroutine(deck, () => completed++));
        }

        // Wait until all coroutines are done
        yield return new WaitUntil(() => completed >= deckList.Count);
    }

    private IEnumerator DeckCoroutine(CharacterDeck deck, System.Action onDone)
    {
        yield return deck.DiscardDrawCoroutine();
        onDone?.Invoke();
    }

    //reset all living units' manas
    private void EndTurnRestoreMana()
    {
        List<Targetable> characters = new List<Targetable>();
        characters.AddRange(TargetingSystem.Instance.allies.members);
        characters.AddRange(TargetingSystem.Instance.enemies.members);

        List<CharacterInfo> infoList = characters
            .Select(c => c.GetComponent<CharacterInfo>())
            .Where(info => info.currentHP > 0)
            .ToList();

        foreach (CharacterInfo character in infoList)
        {
            character.EndTurnRestoreMana();
            character.UpdateResourcesView();
        }
    }
    public int GetManaCost(CardInformation card)
    {
        foreach (Action action in actions)
        {
            if (action.card == card)
            {
                return action.manaCost;
            }
        }
        return 0;
    }

    private int CalculateXP(int charLevel, int enemyLevel)
    {
        int levelDiff = Mathf.Max(enemyLevel, 1);
        int totalXP = 0;

        totalXP += levelDiff * 10;

        return totalXP;
    }
}
