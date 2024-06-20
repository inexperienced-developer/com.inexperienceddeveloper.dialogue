# Inexperienced Developer Dialogue System
A Node-based Dialogue System for Unity. Expanded from the system created by Mert Kirimgeri (https://www.youtube.com/@MertKirimgeriGameDev).

## Install Directly to Unity
_Must make sure that you have git installed_
* Go to Window -> Package Manager
* Click the plus in the top left of the Package Manager Window
* Add package from git URL
* Paste (**CHANGE ME**)
* Click Add

Files can also just be downloaded and dragged into your project.

## HOW TO USE

# Building a graph
* Import package
* On the toolbar click Graph/Dialogue Graph
* To add a node, right-click or space bar
* If you want to add branches, click the Add Branch button
* To remove them you can click the X next to the branch
* Type in desired file name, and save (_Assets are saved to Assets/DialogueTrees_)
* If you want to load your graph, just type in the file name and click load

# Utilizing in game
Once you have a graph you're going to want to use the nodes in game. Luckily it's just a scriptable object (although the data can look a bit messy). The _DialogueUtility_ class is available to traverse the tree.

You can use the _DialogueUtility.GetNextDialogueEvent()_ method to traverse the tree based on selections.

I've included an example of a coroutine-based NPC conversation, just check out _Examples/DialogueUI_.

# Looking to improve
I want to continue updating this for better functionality, more robust features, beautification, as well as performance.

If you have any notes, requests, or some real anger, feel free to email me at: jacob@inexperienceddeveloper.com

## KNOWN ISSUES

# 2024-06-20
* If available - graph should auto-load last opened graph (just annoying otherwise)
* Dialogue text in the nodes doesn't wrap (for the mean time -- pushing enter for a new line and parsing them out at runtime works alright)
