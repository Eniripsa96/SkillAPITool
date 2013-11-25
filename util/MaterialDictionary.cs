﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkillAPITool {

    /// <summary>
    /// Dictionary of bukkit materials
    /// </summary>
    class MaterialDictionary {

        /// <summary>
        /// Valid materials
        /// </summary>
        private static readonly string[] mats = new string[] {
            "ACTIVATOR_RAIL", "AIR", "ANVIL", "APPLE", "ARROW", "BAKED_POTATO", "BEACON", "BED", 
            "BED_BLOCK", "BEDROCK", "BIRCH_WOOD_STAIRS", "BLAZE_POWDER", "BLAZE_ROD", "BOAT", 
            "BONE", "BOOK", "BOOK_AND_QUILL", "BOOKSHELF", "BOW", "BOWL", "BREAD", 
            "BREWING_STAND", "BREWING_STAND_ITEM", "BRICK", "BRICK_STAIRS", "BROWN_MUSHROOM", 
            "BUCKET", "BURNING_FURNACE", "CACTUS", "CAKE", "CAKE_BLOCK", "CARPET", "CARROT", 
            "CARROT_ITEM", "CARROT_STICK", "CAULDRON", "CAULDRON_ITEM", "CHAINMAIL_BOOTS", 
            "CHAINMAIL_CHESTPLATE", "CHAINMAIL_HELMET", "CHAINMAIL_LEGGINGS", "CHEST", "CLAY", 
            "CLAY_BALL", "CLAY_BRICK", "COAL", "COAL_BLOCK", "COAL_ORE", "COBBLE_WALL", 
            "COBBLESTONE", "COBBLESTONE_STAIRS", "COCOA", "COMMAND", "COMPASS", "COOKED_BEEF", 
            "COOKED_CHICKEN", "COOKED_FISH", "COOKIE", "CROPS", "DAYLIGHT_DETECTOR", "DEAD_BUSH", 
            "DETECTOR_RAIL", "DIAMOND", "DIAMOND_AXE", "DIAMOND_BARDING", "DIAMOND_BLOCK", 
            "DIAMOND_BOOTS", "DIAMOND_CHESTPLATE", "DIAMOND_HELMET", "DIAMOND_HOE", 
            "DIAMOND_LEGGINGS", "DIAMOND_ORE", "DIAMOND_PICKAXE", "DIAMOND_SPADE", 
            "DIAMOND_SWORD", "DIODE", "DIODE_BLOCK_OFF", "DIODE_BLOCK_ON", "DIRT", "DISPENSER", 
            "DOUBLE_STEP", "DRAGON_EGG", "DROPPER", "EGG", "EMERALD", "EMERALD_BLOCK", 
            "EMERALD_ORE", "EMPTY_MAP", "ENCHANTED_BOOK", "ENCHANTMENT_TABLE", "ENDER_CHEST", 
            "ENDER_PEARL", "ENDER_PORTAL", "ENDER_PORTAL_FRAME", "ENDER_STONE", "EXP_BOTTLE", 
            "EXPLOSIVE_MINECART", "EYE_OF_ENDER", "FEATHER", "FENCE", "FENCE_GATE", 
            "FERMENTED_SPIDER_EYE", "FIRE", "FIREBALL", "FIREWORK", "FIREWORK_CHARGE", 
            "FISHING_ROD", "FLINT", "FLINT_AND_STEEL", "FLOWER_POT", "FLOWER_POT_ITEM", 
            "FURNACE", "GHAST_TEAR", "GLASS", "GLASS_BOTTLE", "GLOWING_REDSTONE_ORE", 
            "GLOWSTONE", "GLOWSTONE_DUST", "GOLD_AXE", "GOLD_BARDING", "GOLD_BLOCK", 
            "GOLD_BOOTS", "GOLD_CHESTPLATE", "GOLD_HELMET", "GOLD_HOE", "GOLD_INGOT", 
            "GOLD_LEGGINGS", "GOLD_NUGGET", "GOLD_ORE", "GOLD_PICKAXE", "GOLD_PLATE", 
            "GOLD_RECORD", "GOLD_SPADE", "GOLD_SWORD", "GOLDEN_APPLE", "GOLDEN_CARROT", "GRASS", 
            "GRAVEL", "GREEN_RECORD", "GRILLED_PORK", "HARD_CLAY", "HAY_BLOCK", "HOPPER", 
            "HOPPER_MINECART", "HUGE_MUSHROOM_1", "HUGE_MUSHROOM_2", "ICE", "INK_SACK", 
            "IRON_AXE", "IRON_BARDING", "IRON_BLOCK", "IRON_BOOTS", "IRON_CHESTPLATE", 
            "IRON_DOOR", "IRON_DOOR_BLOCK", "IRON_FENCE", "IRON_HELMET", "IRON_HOE", 
            "IRON_INGOT", "IRON_LEGGINGS", "IRON_ORE", "IRON_PICKAXE", "IRON_PLATE", 
            "IRON_SPADE", "IRON_SWORD", "ITEM_FRAME", "JACK_O_LANTERN", "JUKEBOX", 
            "JUNGLE_WOOD_STAIRS", "LADDER", "LAPIS_BLOCK", "LAPIS_ORE", "LAVA", "LAVA_BUCKET", 
            "LEASH", "LEATHER", "LEATHER_BOOTS", "LEATHER_CHESTPLATE", "LEATHER_HELMET", 
            "LEATHER_LEGGINGS", "LEAVES", "LEVER", "LOCKED_CHEST", "LOG", "LONG_GRASS", 
            "MAGMA_CREAM", "MAP", "MELON", "MELON_BLOCK", "MELON_SEEDS", "MELON_STEM", 
            "MILK_BUCKET", "MINECART", "MOB_SPAWNER", "MONSTER_EGG", "MONSTER_EGGS", 
            "MOSSY_COBBLESTONE", "MUSHROOM_SOUP", "MYCEL", "NAME_TAG", "NETHER_BRICK", 
            "NETHER_BRICK_ITEM", "NETHER_BRICK_STAIRS", "NETHER_FENCE", "NETHER_STALK", 
            "NETHER_STAR", "NETHER_WARTS", "NETHERRACK", "NOTE_BLOCK", "OBSIDIAN", "PAINTING", 
            "PAPER", "PISTON_BASE", "PISTON_EXTENSION", "PISTON_MOVING_PIECE", 
            "PISTON_STICKY_BASE", "POISONOUS_POTATO", "PORK", "PORTAL", "POTATO", "POTATO_ITEM", 
            "POTION", "POWERED_MINECART", "POWERED_RAIL", "PUMPKIN", "PUMPKIN_PIE", 
            "PUMPKIN_SEEDS", "PUMPKIN_STEM", "QUARTZ", "QUARTZ_BLOCK", "QUARTZ_ORE", 
            "QUARTZ_STAIRS", "RAILS", "RAW_BEEF", "RAW_CHICKEN", "RAW_FISH", "RECORD_10", 
            "RECORD_11", "RECORD_12", "RECORD_3", "RECORD_4", "RECORD_5", "RECORD_6", "RECORD_7", 
            "RECORD_8", "RECORD_9", "RED_MUSHROOM", "RED_ROSE", "REDSTONE", "REDSTONE_BLOCK", 
            "REDSTONE_COMPARATOR", "REDSTONE_COMPARATOR_OFF", "REDSTONE_COMPARATOR_ON", 
            "REDSTONE_LAMP_OFF", "REDSTONE_LAMP_ON", "REDSTONE_ORE", "REDSTONE_TORCH_OFF", 
            "REDSTONE_TORCH_ON", "REDSTONE_WIRE", "ROTTEN_FLESH", "SADDLE", "SAND", "SANDSTONE", 
            "SANDSTONE_STAIRS", "SAPLING", "SEEDS", "SHEARS", "SIGN", "SIGN_POST", "SKULL", 
            "SKULL_ITEM", "SLIME_BALL", "SMOOTH_BRICK", "SMOOTH_STAIRS", "SNOW", "SNOW_BALL", 
            "SNOW_BLOCK", "SOIL", "SOUL_SAND", "SPECKLED_MELON", "SPIDER_EYE", "SPONGE", 
            "SPRUCE_WOOD_STAIRS", "STAINED_CLAY", "STATIONARY_LAVA", "STATIONARY_WATER", "STEP", 
            "STICK", "STONE", "STONE_AXE", "STONE_BUTTON", "STONE_HOE", "STONE_PICKAXE", 
            "STONE_PLATE", "STONE_SPADE", "STONE_SWORD", "STORAGE_MINECART", "STRING", "SUGAR", 
            "SUGAR_CANE", "SUGAR_CANE_BLOCK", "SULPHUR", "THIN_GLASS", "TNT", "TORCH", 
            "TRAP_DOOR", "TRAPPED_CHEST", "TRIPWIRE", "TRIPWIRE_HOOK", "VINE", "WALL_SIGN", 
            "WATCH", "WATER", "WATER_BUCKET", "WATER_LILY", "WEB", "WHEAT", "WOOD", "WOOD_AXE", 
            "WOOD_BUTTON", "WOOD_DOOR", "WOOD_DOUBLE_STEP", "WOOD_HOE", "WOOD_PICKAXE", 
            "WOOD_PLATE", "WOOD_SPADE", "WOOD_STAIRS", "WOOD_STEP", "WOOD_SWORD", "WOODEN_DOOR", 
            "WOOL", "WORKBENCH", "WRITTEN_BOOK", "YELLOW_FLOWER"
        };

        /// <summary>
        /// Checks if the material name is valid
        /// </summary>
        /// <param name="mat">name to check</param>
        /// <returns>true if valid, false otherwise</returns>
        public static bool IsValid(string mat) {
            mat = convert(mat);
            foreach (string s in mats) {
                if (s.Equals(mat)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts a string into material format
        /// </summary>
        /// <param name="name">string to convert</param>
        /// <returns>material format string</returns>
        public static string convert(string name) {
            return name.ToUpper().Replace(' ', '_');
        }
    }
}
