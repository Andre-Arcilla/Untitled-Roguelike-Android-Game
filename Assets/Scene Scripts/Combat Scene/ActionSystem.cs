using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

            foreach (ICardEffect effect in action.card.card.effects)
            {
                effect.Execute(action.sender, action.card, action.target, action.manaCost);
            }

            int currentHP = targetInfo != null ? targetInfo.currentHP : -100;

            if (targetInfo == null)
            {
                audioSFX = "hit";
            }
            else if (currentHP <= 0 && initialHP > 0)
            {
                audioSFX = "kill";
            }
            else if (currentHP == initialHP)
            {
                audioSFX = "miss";
            }
            else
            {
                audioSFX = "hit";
            }

            yield return ActionPhaseAnimation.Instance.ActionAnimationPerform(action.sender, action.card, action.target, audioSFX);

            yield return senderDeck.EndPlayCard(action.card);
            ActionPhaseAnimation.Instance.ActionAnimationEnd(action.sender, action.card, action.target);

            if (targetInfo != null && targetInfo.currentHP <= 0)
            {
                targetInfo.currentHP = 0;
                targetInfo.UpdateResourcesView();

                if (TargetingSystem.Instance.allies.members.Contains(senderInfo.GetComponent<Targetable>()))
                {
                    int levelDiff = Mathf.Max(targetInfo.characterData.basicInfo.level - senderInfo.characterData.basicInfo.level, 1);
                    float multiplier = Mathf.Max(levelDiff * 0.75f, 1.25f);
                    int goldFound = Mathf.RoundToInt(Random.Range(50, 101) * multiplier);

                    CombatSystem.Instance.AddKillCount(senderInfo.characterData, levelDiff, goldFound);
                }
                //targetInfo.gameObject.SetActive(false);
            }

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

        List<CharacterInfo> infoList = characters
            .Select(c => c.GetComponent<CharacterInfo>())
            .ToList();

        foreach (CharacterInfo character in infoList)
        {
            character.OnTurnStart();
        }

        foreach (CharacterInfo character in infoList)
        {
            character.OnTurnEnd();
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
            Debug.Log("All allies are dead. Defeat.");
            CombatSystem.Instance.CombatCompleteLose(allAlliesDead);
        }
        else if (allEnemiesDead == true)
        {
            Debug.Log("All enemies are dead. Victory!");
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

        CharacterDeck senderDeck = sender.GetComponent<CharacterDeck>();

        yield return StartCoroutine(senderDeck.StartPlayCard(card));

        foreach (ICardEffect effect in card.card.effects)
        {
            effect.Execute(sender, card, target, card.card.mana);
        }

        target.GetComponent<CardInformation>().UpdateCard();

        yield return new WaitForSeconds(0.5f);

        yield return senderDeck.EndPlayCard(card);

        yield return senderDeck.EndPlaySortHand();

        yield return senderDeck.UpdateSelectedCardPos();

        CharacterGenerator.Instance.EnablePlayerRaycasts();
    }

    //discards and draws all living units' decks
    private IEnumerator DiscardAndDrawAllDecks()
    {
        List<Targetable> characters = new List<Targetable>();
        characters.AddRange(TargetingSystem.Instance.allies.members);
        characters.AddRange(TargetingSystem.Instance.enemies.members);

        List<CharacterDeck> deckList = characters
            .Where(c => c.GetComponent<CharacterInfo>().currentHP > 0)
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
}
