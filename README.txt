First of all, thank you for the exciting challenge project. I spent about 30 hours in total finishing all of this. I will write down the following things in this file.

1. Brief summaries of what AI algorithm I used for the opponent AI
2. The change I made to the animated background code
3(a) How I would refactor the code and/or project
3(b) How I would make this game online
3(c) How I would modify the existing code to be able to make versions of the game that use any other polygons

Some figures that should be helpful to understand my implementation are included in the zip folder as a pdf file named Figures.pdf. Also, the execution file resides in Sim/Build/Sim.exe.

1. Brief summaries of what AI algorithm I used for the opponent AI

I used the Min-Max search algorithm, which is named MinMax() in GameState class. This is basically to keep choosing the choice that minimizes the maximum loss using a recursive function. However, with this algorithm alone, the performance was not enough for the game to be playable.
Here are the techniques I used in this project to further improve the performance.
(1) alpha-beta pruning
During the search, if AI is trying to find the minimum score, it is no use trying to search the descendant nodes once it finds a node that has a bigger score than any node it has seen. This is called beta pruning. 
Similarly, during the search, if AI is trying to find the maximum score, it is no use trying to search the descendant nodes once it finds a node that has a smaller score than any node it has seen. This is called alpha pruning. 
By using the alpha-beta pruning technique, I could reduce the number of searches.

(2) memoization for recursive function
I used memoization to speed up the performance. The variable named statesMemo in GameState class is a dictionary that has the (key, value) pair of (state ID, (AI's score, next best action)). By memorizing all the scores that were once calculated, it can avoid recalculating the same thing. 
State ID was calculated using a ternary system. Each line was marked as 0 (not selected), 1 (selected by AI), or -1 (selected by a human) and the state ID is the decimal conversion of the ternary representation. This way, any state corresponds with one unique ID (-7174453 ~ 7174453).

(3) Rotation and Mirroring when registering to the memo
Whenever AI finds a score for a state, it rotates and mirrors the state to get all the symmetrical states at the same time. Since the board shape is symmetrical, many states are identical if we rotate (figure 1) or mirror (figure 2). Note that we can rotate 6 times to get the different states, and we have three axes to mirror to get the different states.
 I assumed that one state can have at most 6 (rotations) * (3 + 1) (mirroring including original state) = 24 identical states. This should also reduce the number of re-calculations of the identical states.

(4) Limited number of choices for the first AI choice
In the GetRemainingLineSegments() method in GameState class, if it is AI's first turn, it outputs only a limited amount of choices to reduce the number of searches by removing the symmetric choices. (Figures 3, 4, 5)
For example, if the player chooses line 2 (Figure 3), it is enough to check the five lines 6, 9, 10, 11, and 14 because other edges are equivalent to one of them.

2. The change I made to the animated background code

I utilized Junior's implementation as much as possible. I created an inherited class named MainGameTextureCreator, and inside the inherited class, I used the time.deltaTime to make some of the parameters keep changing over time. The lacunarity value changes smoothly using the trigonometric function, and the dimensions value changes discretely (1->2->3->2->1->2->3...) every 3 seconds.
I didn't bother to change other parameters because it might degrade the user experience.

3(a). How I would refactor the code and/or project
[Must happen]
(1) AI calculation should be an asynchronous operation 
Since the Min-Max algorithm includes a heavy calculation and if it is inside the Update method, that would mean it could stop the frame including the animated background, which would lead to an absolutely bad user experience. (I already used UniTask in PolygonSide class to implement it asynchronously.)

(2) Use other ways to speed up AI calculation
The first AI choice still takes about 5 seconds on my computer (Intel Core i7 1.30GHz 8CPUs) and it needs to further speed up for a better user experience. My idea is that (i) precompute the statesMemo dictionary by calling an asynchronous function during the main menu scene, or even (ii) call the Min-Max function before playing to create a complete stateMemo and output the data to some text file. Then the system reads it when it launches.

(3) Refactor GameState
Currently, it is a bit hard to access the GameState class from the scene, so I would put an empty GameObject named GameManager that has GameState.cs on it. It should make the developer easier to access GameState all the time. Also, GameState class should have a property to access all the LineSegments in the scene.

[Nice to have]
(1) Add some visual effect when AI chooses / Delay AI choice
In this version, sometimes it is a bit hard for the player to identify which line AI chose. By adding some fun effects, it would be more enjoyable. Also, the fact that the AI's choice is sometimes too fast leads to a bad user experience, so I would delay the AI's choice to at least 1 second to make the player feel like they are actually "playing with" the opponent.

(2) Add some animation while AI is thinking
I already added the text saying "AI IS THINKING..." while AI is thinking, but adding some fun animation would be more enjoyable.

(3) Improve DoLineSegmentsFormTriangle() method
In the current DoLineSegmentsFormTriangle() in PlayerState class, it is not fully optimized because it checks all the possible patterns for the first edge. This is unnecessary because if a triangle was formed, one of the edges has to be the last line selected, so by using the last line as an argument, we could speed up this method.

(4) Make AI be non-deterministic
Currently, AI acts deterministically - AI chooses the same line given the same state. It would not be fun to play if AI does not have some sort of randomness. For example, we can either do (i)GetRemainingLineSegments() outputs only randomly selected lines, and the best choice among them will be chosen or (ii)AI chooses random choices in the first few turns.

(5) Let the player choose the difficulty
It would be fun if the player can choose AI's smartness. To do this, for example, we can let GetRemainingLineSegments() output all the possible lines in hard mode, output half of the possible lines in medium mode, and output only the first possible line in easy mode.

(6) Set all Hexagon Line objects under a parent object
It is a little bit messy if the hexagon lines are directly under the scene. I would set all of them under a parent object to make it less messy.

3(b) How I would make this game online
I would use Photon Fusion shared mode to make an online version of this game. 
The high-level steps would be to add a network manager object and perform both players' choices under NetworkBehaviour class instead of MonoBehaviours class. The state can be created in each client's environment given both players' choices. So the state does not have to be shared. Also, it would be nice to create a lobby scene to wait for the other player to join.

The protocol is going to be TCP because this game does not require real-time interaction, and some lag is not that big of an issue. Instead, the player's choice should be transmitted to the server and all clients precisely.
With shared mode in Photon Fusion, the authority over the objects is distributed across all the clients. This could also ensure data validation and prevents the client from cheating, which is also great.

3(c) How I would modify the existing code to be able to make versions of the game that use any other polygons
There are several steps to take. (i)Add other polygons into the Enum BoardShape. (ii)Add BoardShape information to an argument of TakeAITurn(). (iii)Change GetRemainingLineSegments() method so that it could work for other polygons, too. Now GetRemainingLineSegments() is only developed for a hexagon. (iv)RegisterInMemo() is only designed for a hexagon as well, so it needs to be changed so that it rotates n times and mirrors [n/2] + 1 times for an n-sided polygon. (v)mappings in Calculations class should be redesigned

Even after doing all of them, it is obvious that the speed will be a problem if we make an n-sided (n > 6) polygon, so as I discussed above, we should do precomputation or introduce some randomness to improve the performance.

This is the end of the document. Thank you for reading!