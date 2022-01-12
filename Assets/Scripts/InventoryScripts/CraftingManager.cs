﻿using System.Collections.Generic;
using System.Linq;
using InventoryScripts.ItemScripts;
using UnityEngine;

namespace InventoryScripts
{
    public class CraftingManager : MonoBehaviour
    {
        [SerializeField] private Transform resultSlot;
        [SerializeField] private Transform ingredientsParent;
        [SerializeField] private Transform craftableParent;
        [SerializeField] private Transform[] ingredientsSlots;
        [SerializeField] private CraftableSlot[] craftableSlots;
        public CraftingRecipe[] craftingRecipes;
        private List<CraftingRecipe> _craftable;
        private List<Item> _ingredientSlotItems;
        private CraftingRecipe _crafting;

        
        private void Awake()
        {
            _ingredientSlotItems = new List<Item>();
            var craftingObjects = Resources.LoadAll("Crafting Recipes");
            craftingRecipes = new CraftingRecipe[craftingObjects.Length];
            craftingObjects.CopyTo(craftingRecipes, 0);
            craftableSlots = craftableParent.GetComponentsInChildren<CraftableSlot>();
            ingredientsSlots = new Transform[ingredientsParent.childCount];
            for (int i = 0; i < ingredientsParent.childCount; i++)
                ingredientsSlots[i] = ingredientsParent.GetChild(i);
        }

        private void Start() {
            Inventory.OnInventoryUpdated += UpdateRecipes;
        }

        private void OnDestroy() {
            Inventory.OnInventoryUpdated -= UpdateRecipes;
        }

        private void OnEnable() {
            UpdateRecipes();
        }
        
        
        public void ShowRecipe(int index)
        {
            foreach (var slot in ingredientsSlots) 
                ClearIngredientsSlot(slot);
            
            
            _crafting = craftableSlots[index].myRecipe;
            if (_crafting == null) return;
            //Show result
            resultSlot.GetComponent<InventorySlot>().MyItem =
                new Item(_crafting.result.itemData, _crafting.result.amount);
            //Show ingredients
            var ingredients = _crafting.ingredients;
            for (int i = 0; i < ingredientsSlots.Length; i++)
            {
                if (i >= ingredients.Length) {
                    ClearIngredientsSlot(ingredientsSlots[i]);
                    continue;
                }
                var currIngredient = Inventory.Instance.itemList.Find(item => 
                    item.itemData == ingredients[i].itemData && item.Amount >= ingredients[i].amount);
                //Case1: we have just the right amount of the ingredient move the item from inventory to crafting slot
                if (currIngredient.Amount == ingredients[i].amount)
                {
                    var invSlot = Inventory.Instance.GetSlotFromItem(currIngredient);
                    var craftSlot = ingredientsSlots[i].GetComponent<InventorySlot>();
                    var tempItem = invSlot.MyItem;
                    invSlot.MyItem = craftSlot.MyItem;
                    craftSlot.MyItem = tempItem;
                }
                //Case2: we have a surplus of the ingredient move the amount we need to the crafting slot
                else
                {
                    currIngredient.Amount -= ingredients[i].amount;
                    var splitItem = new Item(currIngredient.itemData, ingredients[i].amount);
                    ingredientsSlots[i].GetComponent<InventorySlot>().MyItem = splitItem;
                }
            }
        }

        private void ClearIngredientsSlot(Transform iSlotParent)
        {
            var iSlot = iSlotParent.GetComponentInChildren<InventorySlot>();
            if (iSlot.MyItem == null) return;
            var existingItemStack = Inventory.Instance.itemList.FirstOrDefault(
                item =>
                    iSlot &&
                    iSlot.MyItem?.itemData == item.itemData
                    && iSlot.MyItem != item
            );
            if (existingItemStack != null)
                existingItemStack.Amount += iSlot.MyItem.Amount;
            else
                Inventory.Instance.AddItem(iSlot.MyItem);
            iSlot.MyItem = null;
        }

        private void UpdateRecipes()
        {
            _craftable = craftingRecipes.Where(cr => RecipeCraftable(cr, Inventory.Instance.itemList)).ToList();
            for (int i = 0; i < craftableSlots.Length; i++)
            {
                if (i < _craftable.Count) {
                    craftableSlots[i].img.sprite = _craftable[i].result.itemData.itemSprite;
                    craftableSlots[i].amountText.text =_craftable[i].result.amount > 1 ? $"{_craftable[i].result.amount}" : "";
                    craftableSlots[i].myRecipe = _craftable[i];
                    continue;
                }
                craftableSlots[i].img.sprite = null;
                craftableSlots[i].myRecipe = null;
                craftableSlots[i].amountText.text = "";
            }
            CheckForCraftingResult();
        }

        private bool RecipeCraftable(CraftingRecipe cr, List<Item> itemList)
        {
            foreach (var ingredient in cr.ingredients)
            {
                if (!itemList.Any(item => 
                    item.Amount >= ingredient.amount && item.itemData == ingredient.itemData)) {
                    return false;
                }
            }
            return true;
        }

        //called by button press on results slot
        public void UseIngredients()
        {
            if (_crafting == null) return;
            for (int i = 0; i < ingredientsSlots.Length; i++)
            {
                var slotItem = ingredientsSlots[i].GetComponentInChildren<InventorySlot>();
                if (slotItem.MyItem == null) continue;
                    if (i >= _crafting.ingredients.Length || slotItem.MyItem.Amount == _crafting.ingredients[i].amount)
                {
                    Inventory.Instance.itemList.Remove(slotItem.MyItem);
                    slotItem.GetComponentInChildren<InventorySlot>().MyItem = null;
                }
                else 
                {
                    slotItem.MyItem.Amount -= _crafting.ingredients[i].amount;
                    ClearIngredientsSlot(slotItem.transform.parent);
                }
            }
            _crafting = null;
        }

        private void CheckForCraftingResult()
        {
            _ingredientSlotItems.Clear();
            foreach (var slot in ingredientsSlots)
            {
                var slotItem = slot.GetComponentInChildren<InventorySlot>().MyItem;
                if (slotItem != null)
                    _ingredientSlotItems.Add(slotItem);
            }
            var recipe = _craftable.Find(cr => RecipeCraftable(cr, _ingredientSlotItems) 
                                               && cr.ingredients.Length == _ingredientSlotItems.Count);
            if (recipe != null)
            {
                _crafting = recipe;
                resultSlot.GetComponentInChildren<InventorySlot>().MyItem =
                    new Item(recipe.result.itemData, recipe.result.amount);
            }
            else
            {
                _crafting = null;
                if (resultSlot.GetComponentInChildren<InventorySlot>())
                    resultSlot.GetComponentInChildren<InventorySlot>().MyItem = null;
            }
        }

        private void OnDisable()
        {
            foreach (var slot in ingredientsSlots)
                ClearIngredientsSlot(slot);
        }
    }
}