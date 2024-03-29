﻿using System;
using System.Collections.Generic;
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
        private CraftingRecipe.CraftType _craftableType;
        
        private void Awake()
        {
            _craftableType = CraftingRecipe.CraftType.Hands;
            Inventory.OnInventoryUpdated += UpdateRecipes;
            CraftingStation.craftTypeChange += UpdateCraftType;
            _ingredientSlotItems = new List<Item>();
            var craftingObjects = Resources.LoadAll("Crafting Recipes");
            craftingRecipes = new CraftingRecipe[craftingObjects.Length];
            craftingObjects.CopyTo(craftingRecipes, 0);
            craftableSlots = craftableParent.GetComponentsInChildren<CraftableSlot>();
            ingredientsSlots = new Transform[ingredientsParent.childCount];
            for (int i = 0; i < ingredientsParent.childCount; i++)
                ingredientsSlots[i] = ingredientsParent.GetChild(i);
        }

        private void OnDestroy() {
            Inventory.OnInventoryUpdated -= UpdateRecipes;
            CraftingStation.craftTypeChange -= UpdateCraftType;
        }

        private void OnEnable() 
        {
            UpdateRecipes();
        }
        
        
        public void ShowRecipe(int index)
        {
            if (craftableSlots[index].myRecipe == null || _crafting ==  craftableSlots[index].myRecipe) return;
            foreach (var slot in ingredientsSlots) 
                ClearIngredientsSlot(slot);
            _crafting = craftableSlots[index].myRecipe;
            //Show result
            resultSlot.GetComponent<InventorySlot>().MyItem =
                new Item(_crafting.result.itemData, _crafting.result.amount);
            //Show ingredients
            var ingredients = _crafting.ingredients;
            if (_crafting.useAnyIngredient)
            {
                foreach (var ingredient in _crafting.ingredients)
                {
                    if (Inventory.Instance.itemList.Any(item => 
                        item.Amount >= ingredient.amount && item.itemData == ingredient.itemData)) {
                        ShowIngredient(ingredient, 1);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ingredients.Length; i++)
                    ShowIngredient(ingredients[i], i);
            }
        }

        private void ShowIngredient(CraftingRecipe.ItemAmountPair ingredient, int slotNumber)
        {
            var currIngredient = Inventory.Instance.itemList.Find(item => 
                item.itemData == ingredient.itemData && item.Amount >= ingredient.amount);
            //Case1: we have just the right amount of the ingredient move the item from inventory to crafting slot
            if (currIngredient.Amount == ingredient.amount)
            {
                var invSlot = Inventory.Instance.GetSlotFromItem(currIngredient);
                var craftSlot = ingredientsSlots[slotNumber].GetComponent<InventorySlot>();
                var tempItem = invSlot.MyItem;
                invSlot.MyItem = craftSlot.MyItem;
                craftSlot.MyItem = tempItem;
                /*
                Inventory.Instance.itemList.Remove(currIngredient);
                if (craftSlot.MyItem != null)
                    Inventory.Instance.itemList.Add(craftSlot.MyItem);
                */
            }
            //Case2: we have a surplus of the ingredient move the amount we need to the crafting slot
            else
            {
                currIngredient.Amount -= ingredient.amount;
                var splitItem = new Item(currIngredient.itemData, ingredient.amount);
                ingredientsSlots[slotNumber].GetComponent<InventorySlot>().MyItem = splitItem;
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
            {
                var emptySlot = Inventory.Instance.slotList.FirstOrDefault(slot => slot.MyItem == null);
                if (emptySlot) 
                    emptySlot.MyItem = iSlot.MyItem;
                iSlot.MyItem = null;
            }
            /*
            else
                Inventory.Instance.AddItem(iSlot.MyItem);
            */
            iSlot.MyItem = null;
        }

        private void UpdateRecipes()
        {
            _craftable = craftingRecipes.Where(cr => RecipeCraftable(cr, Inventory.Instance.itemList)).ToList();
            for (int i = 0; i < craftableSlots.Length; i++)
            {
                if (i < _craftable.Count) {
                    craftableSlots[i].SlotSprite = _craftable[i].result.itemData.itemSprite;
                    craftableSlots[i].amountText.text =_craftable[i].result.amount > 1 ? $"{_craftable[i].result.amount}" : "";
                    craftableSlots[i].myRecipe = _craftable[i];
                    continue;
                }
                craftableSlots[i].SlotSprite = null;
                craftableSlots[i].myRecipe = null;
                craftableSlots[i].amountText.text = "";
            }
            CheckForCraftingResult();
        }

        private void UpdateCraftType(CraftingRecipe.CraftType newCraftType)
        {
            _craftableType = newCraftType;
            UpdateRecipes();
        }

        private bool RecipeCraftable(CraftingRecipe cr, List<Item> itemList)
        {
            if (cr.craftWith != _craftableType) return false;
            if (cr.useAnyIngredient)
            {
                foreach (var ingredient in cr.ingredients)
                {
                    if (itemList.Any(item => 
                        item.Amount >= ingredient.amount && item.itemData == ingredient.itemData)) {
                        return true;
                    }
                }
                return false;
            }
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
            var recipe = craftingRecipes.FirstOrDefault(cr => RecipeCraftable(cr, _ingredientSlotItems));
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