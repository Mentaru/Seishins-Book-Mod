using System;
using System.Collections;
using BepInEx;
using BepInEx.Configuration;

using UnityEngine;


namespace SeishinsBookMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        private readonly ConfigEntry<bool> _enabled;
        private readonly ConfigEntry<bool> _bookRequirements;
        private readonly ConfigEntry<bool> _noBooksNoRequirements;
        private readonly ConfigEntry<bool> _bugBookEnabled;
        private readonly ConfigEntry<bool> _fishBookEnabled;
        private readonly ConfigEntry<bool> _plantBookEnabled;
        private readonly ConfigEntry<bool> _allBooksEnabled;
        private readonly ConfigEntry<KeyCode> _bugBookHotKey;
        private readonly ConfigEntry<KeyCode> _fishBookHotKey;
        private readonly ConfigEntry<KeyCode> _plantBookHotKey;
        private readonly ConfigEntry<KeyCode> _allBooksHotKey;

        private bool runOnce;

        public Plugin()
        {
            _enabled = Config.Bind("Mod Toggle", "Enabled", true, "Set to true/false to enabled and disable the mod");
            _bookRequirements = Config.Bind("Options", "BookRequirements", false, "Set to true if you wish to be able to use the books without them in your inventory but still need to have the requirements to buy the books (45 combined total of Fish/Bugs, 2 Hearts with Rayne)");
            _noBooksNoRequirements = Config.Bind("Options", "NoBooksNoRequirements", false, "Set to true if you want to use this mod without the books in inventory and don't need the ability to buy them");
            _bugBookEnabled = Config.Bind("Toggles", "BugBookEnabled", false, "Default is set to false, The hotkey will swap between true/false, If you set to true it will display the information when loaded");
            _fishBookEnabled = Config.Bind("Toggles", "FishBookEnabled", false, "Default is set to false, The hotkey will swap between true/false, If you set to true it will display the information when loaded");
            _plantBookEnabled = Config.Bind("Toggles", "PlantBookEnabled", false, "Default is set to false, The hotkey will swap between true/false, If you set to true it will display the information when loaded");
            _allBooksEnabled = Config.Bind("Toggles", "AllBooksEnabled", false, "Default is set to false, The hotkey will swap between true/false, If you set to true it will display the information when loaded");
            _bugBookHotKey = Config.Bind("Key Binds", "BugBookHotKey", KeyCode.LeftBracket, "The Keybind that will toggle if the Bug Book is active, Press once to activate, press again to deactivate");
            _fishBookHotKey = Config.Bind("Key Binds", "FishBookHotKey", KeyCode.RightBracket, "The Keybind that will toggle if the Fish Book is active, Press once to activate, press again to deactivate");
            _plantBookHotKey = Config.Bind("Key Binds", "PlantBookHotKey", KeyCode.Minus, "The Keybind that will toggle if the Plant Book is active, Press once to activate, press again to deactivate");
            _allBooksHotKey = Config.Bind("Key Binds", "AllBooksHotKey", KeyCode.Equals, "The Keybind that will toggle if all the Books are active, Will display the information of each book you have in your inventory (If BookRequirements is true, Will activate the books you meet the requirements for), Press once to activate, press again to deactivate");

        }

        private void Awake()
        {
            // Plugin startup logic
            if (_enabled.Value == true)
            {
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            }
            else
            {
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is disabled!");
                Logger.LogInfo("You can enable it in Seishin.BookMod.cfg file located in C:\\Program Files (x86)\\Steam\\steamapps\\common\\Dinkum\\BepInEx\\config");
            }
        }

        public bool hasBugBook;
        public bool hasFishBook;
        public bool hasPlantBook;
        public bool hasAllBooks;
        public bool bookRequirementsMet;
        public bool plantBookRequirementsMet;
        private void BookCheck()
        {
            foreach (InventorySlot inventorySlot in Inventory.inv.invSlots)
            {
                if (_bookRequirements.Value == true)
                {
                    CheckBookRequirements();
                }
                if (inventorySlot.itemNo == 679 || bookRequirementsMet || _noBooksNoRequirements.Value)
                {
                    hasBugBook = true;
                }
                if (inventorySlot.itemNo == 680 || bookRequirementsMet || _noBooksNoRequirements.Value)
                {
                    hasFishBook = true;
                }
                if (inventorySlot.itemNo == 681 || plantBookRequirementsMet || _noBooksNoRequirements.Value)
                {
                    hasPlantBook = true;
                }
            }
            if (hasBugBook && hasFishBook && hasPlantBook)
            {
                hasAllBooks = true;
            }
        }

        private void CheckBookRequirements()
        {
            int fish = MuseumManager.manage.allFish.Count;
            int bugs = MuseumManager.manage.allBugs.Count;
            int totalDonated = fish + bugs;

            if (totalDonated >= 45)
            {
                bookRequirementsMet = true;
            }

            if (NPCManager.manage.npcStatus[0].relationshipLevel >= 40)
            {
                plantBookRequirementsMet = true;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(_bugBookHotKey.Value) && _enabled.Value)
            {
                BookCheck();
                if (hasBugBook)
                {
                    _bugBookEnabled.Value = !_bugBookEnabled.Value;
                    NotificationManager.manage.createChatNotification($"Your bug book is now {(_bugBookEnabled.Value ? "enabled" : "disabled")}.");
                    Config.Save();
                    hasBugBook = false;

                    if (_bugBookEnabled.Value)
                    {
                        AnimalManager.manage.bugBookOpen = true;
                        AnimalManager.manage.lookAtBugBook.Invoke();
                    }
                    else
                    {
                        AnimalManager.manage.bugBookOpen = false;
                        AnimalManager.manage.lookAtBugBook.Invoke();
                    }
                }
                else
                {
                    if (_bookRequirements.Value)
                    {
                        NotificationManager.manage.createChatNotification($"You do not meet the requirements to use the bug book");
                    }
                    else
                    {
                        NotificationManager.manage.createChatNotification($"You must have a bug book to use this setting");
                    }
                }
            }

            if (Input.GetKeyDown(_fishBookHotKey.Value) && _enabled.Value)
            {
                BookCheck();
                if (hasFishBook)
                {
                    _fishBookEnabled.Value = !_fishBookEnabled.Value;
                    NotificationManager.manage.createChatNotification($"Your fish book is now {(_fishBookEnabled.Value ? "enabled" : "disabled")}.");
                    Config.Save();
                    hasFishBook = false;

                    if (_fishBookEnabled.Value)
                    {
                        AnimalManager.manage.fishBookOpen = true;
                        AnimalManager.manage.lookAtFishBook.Invoke();
                    }
                    else
                    {
                        AnimalManager.manage.fishBookOpen = false;
                        AnimalManager.manage.lookAtFishBook.Invoke();
                    }
                }
                else
                {
                    if (_bookRequirements.Value)
                    {
                        NotificationManager.manage.createChatNotification($"You do not meet the requirements to use the fish book");
                    }
                    else
                    {
                        NotificationManager.manage.createChatNotification($"You must have a fish book to use this setting");
                    }
                }
            }

            if (Input.GetKeyDown(_plantBookHotKey.Value) && _enabled.Value)
            {
                BookCheck();
                if (hasPlantBook)
                {
                    _plantBookEnabled.Value = !_plantBookEnabled.Value;
                    NotificationManager.manage.createChatNotification($"Your plant book is now {(_plantBookEnabled.Value ? "enabled" : "disabled")}.");
                    Config.Save();
                    hasPlantBook = false;

                    if(!_plantBookEnabled.Value)
                    {
                        return;
                    } 
                    else
                    {
                        StartCoroutine(this.plantBookRoutine());
                    }
                }
                else
                {
                    if (_bookRequirements.Value)
                    {
                        NotificationManager.manage.createChatNotification($"You do not meet the requirements to use the plant book");
                    }
                    else
                    {
                        NotificationManager.manage.createChatNotification($"You must have a plant book to use this setting");
                    }
                }
            }

            if (Input.GetKeyDown(_allBooksHotKey.Value) && _enabled.Value)
            {
                BookCheck();
                if (hasAllBooks || hasBugBook || hasFishBook || hasPlantBook)
                {
                    _allBooksEnabled.Value = !_allBooksEnabled.Value;
                    NotificationManager.manage.createChatNotification($"All your books are now {(_allBooksEnabled.Value ? "enabled" : "disabled")}.");
                    Config.Save();
                    hasAllBooks = false;

                    if (_allBooksEnabled.Value)
                    {
                        if (!_bugBookEnabled.Value && hasBugBook)
                        {
                            _bugBookEnabled.Value = true;
                            hasBugBook = false;
                            AnimalManager.manage.bugBookOpen = true;
                            AnimalManager.manage.lookAtBugBook.Invoke();
                        }

                        if (!_fishBookEnabled.Value && hasFishBook)
                        {
                            _fishBookEnabled.Value = true;
                            hasFishBook = false;
                            AnimalManager.manage.fishBookOpen = true;
                            AnimalManager.manage.lookAtFishBook.Invoke();
                        }

                        if (!_plantBookEnabled.Value && hasPlantBook)
                        {
                            _plantBookEnabled.Value = true;
                            hasPlantBook = false;
                            StartCoroutine(this.plantBookRoutine());
                        }
                    }
                    else
                    {
                        if (_bugBookEnabled.Value)
                        {
                            _bugBookEnabled.Value = false;
                            AnimalManager.manage.bugBookOpen = false;
                            AnimalManager.manage.lookAtBugBook.Invoke();
                        }
                        
                        if (_fishBookEnabled.Value)
                        {
                            _fishBookEnabled.Value = false;
                            AnimalManager.manage.fishBookOpen = false;
                            AnimalManager.manage.lookAtFishBook.Invoke();
                        }

                        if (_plantBookEnabled.Value)
                        {
                            _plantBookEnabled.Value = false;
                        }
                    }
                }
                else
                {
                    if (_bookRequirements.Value)
                    {
                        NotificationManager.manage.createChatNotification($"You do not meet any requirements to activate a book");
                    }
                    else
                    {
                        NotificationManager.manage.createChatNotification($"You have no books to use this setting");
                    }
                }
            }

            if (_enabled.Value && !NetworkMapSharer.share.localChar)
            {
                if (runOnce)
                {
                    return;
                }
                _plantBookEnabled.Value = false;
                runOnce = true;
            }

            if (_enabled.Value && runOnce)
            {
                runOnce = false;
                refreshModOnWorldEntry();
            }
        }

        private void refreshModOnWorldEntry()
        {
            if (_bugBookEnabled.Value)
            {
                AnimalManager.manage.bugBookOpen = true;
                AnimalManager.manage.lookAtBugBook.Invoke();
            }

            if (_fishBookEnabled.Value)
            {
                AnimalManager.manage.fishBookOpen = true;
                AnimalManager.manage.lookAtFishBook.Invoke();
            }

            if (_plantBookEnabled.Value)
            {
                StartCoroutine(this.plantBookRoutine());
            }
        }

        private IEnumerator plantBookRoutine()
        {
            int lastXpos = -1;
            int lastYpos = -1;
            for (; ; )
            {
                if (!_plantBookEnabled.Value)
                {
                    if (NetworkMapSharer.share.localChar)
                    {
                        BookWindow.book.closeBook();
                    }
                    while (!_plantBookEnabled.Value)
                    {
                        yield return null;
                    }
                }
                else
                {
                    int num = Mathf.RoundToInt(NetworkMapSharer.share.localChar.myInteract.tileHighlighter.transform.position.x / 2f);
                    int num2 = Mathf.RoundToInt(NetworkMapSharer.share.localChar.myInteract.tileHighlighter.transform.position.z / 2f);
                    if (lastXpos != num || lastYpos != num2)
                    {
                        if (WorldManager.manageWorld.onTileMap[num, num2] >= 0 && WorldManager.manageWorld.allObjects[WorldManager.manageWorld.onTileMap[num, num2]].tileObjectGrowthStages && WorldManager.manageWorld.allObjects[WorldManager.manageWorld.onTileMap[num, num2]].tileObjectGrowthStages.isPlant && !WorldManager.manageWorld.allObjects[WorldManager.manageWorld.onTileMap[num, num2]].tileObjectGrowthStages.normalPickUp)
                        {
                            TileObjectGrowthStages tileObjectGrowthStages = WorldManager.manageWorld.allObjects[WorldManager.manageWorld.onTileMap[num, num2]].tileObjectGrowthStages;
                            if (tileObjectGrowthStages.harvestDrop)
                            {
                                BookWindow.book.objectTitle.text = tileObjectGrowthStages.harvestDrop.getInvItemName() + " Plant";
                                if (tileObjectGrowthStages.objectStages.Length - WorldManager.manageWorld.onTileStatusMap[num, num2] - 1 == 0)
                                {
                                    BookWindow.book.openPlantBook("Ready for harvest");
                                }
                                else
                                {
                                    BookWindow.book.openPlantBook("Mature in:\n" + (tileObjectGrowthStages.objectStages.Length - WorldManager.manageWorld.onTileStatusMap[num, num2] - 1).ToString() + " days.");
                                }
                            }
                            else
                            {
                                BookWindow.book.objectTitle.text = "Plant";
                                if (tileObjectGrowthStages.objectStages.Length - WorldManager.manageWorld.onTileStatusMap[num, num2] - 1 == 0)
                                {
                                    BookWindow.book.openPlantBook("");
                                }
                                else
                                {
                                    BookWindow.book.openPlantBook("Mature in:\n" + (tileObjectGrowthStages.objectStages.Length - WorldManager.manageWorld.onTileStatusMap[num, num2] - 1).ToString() + " days");
                                }
                            }
                        }
                        else
                        {
                            BookWindow.book.closeBook();
                        }
                    }
                        yield return null;
                }
            }
        }
    }
}
