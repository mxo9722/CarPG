_________________________________________________________________________________________________________

Package "Tail Animator"
Version 1.3.6.2

Made by FImpossible Creations - Filip Moeglich
https://www.FilipMoeglich.pl
FImpossibleGames@Gmail.com or Filip.Moeglich@Gmail.com

_________________________________________________________________________________________________________

Unity Connect: https://connect.unity.com/u/5b2e9407880c6425c117fab1
Youtube: https://www.youtube.com/channel/UCDvDWSr6MAu1Qy9vX4w8jkw
Facebook: https://www.facebook.com/FImpossibleCreations
Twitter (@FimpossibleC): https://twitter.com/FImpossibleC
Google+: https://plus.google.com/u/3/115325467674876785237

_________________________________________________________________________________________________________

If you will not need demo scenes anymore, you cam remove folders like:
Assets/FImpossible Creations/Tail Animator/Demo - Tail Animator  (Here are demo scenes and demo script examples also demo models and prefabs)
Assets/FImpossible Creations/Ground Fitter  (From here is only terrain asset for tail examples)
Assets/FImpossible Creations/FBasic Assets/ All folders but NOT 'Editor' 'Editor Tools' 'Scripts' 'FHelpers'  
(From here you will remove different scripts used in demo scenes but leave utility ones needed for tail animator to work)
Then package would have ~2.5mb weight
_________________________________________________________________________________________________________
Description:

Tail Animator is package of behaviors simulating elastic tail movement.
Animation is procedural so it reacts with changes on object's position / rotation / scale.
Just add component to object you want to animate elastic, define start transform (or bone) and play with 
parameters, but remember that bones rotations orientation should be correct with unity axes.
Scripts are commented in detailed way.
You also can put component on animated meshes and blend procedural animation with animator's movement.

Main features:
-Includes interactive demo scenes
-Includes full source code
-Scripts are commented in detailed way
-Visual friendly inspector window

Main Components:
- FTail_Animator - Component to animate transforms like tail
- FTail_AnimatorBlending - Component dedicated for animated objects with possibility to blend animation with tail animator
- FTail_Animator2D / UI - Components to animate tail behavior in 2D space
- FTail_Editor_Skinner - Components to create skinned model ready to be animated from static mesh, all inside unity

_________________________________________________________________________________________________________
Version history:

v 1.3.6.2
- Added new animation parameter "Motion Influence" - if your character moves too fast you can controll influence of position changes for tail motion.

v1.3.6.1
- Added "Animate Root" option, which is applying animation rotations of first chain in bone like "Animate Corrections"
- Small changes and fixes inside inspector window

v1.3.6
- Added "Max Angle" variable to limit maximum rotation of each segment from it's initial rotation
- Using "Max Angle" parameter, enables other parameters to limit selective axis of rotating, otherwise all angles are clamped to "Max Angle"
- Added parameter "Stiff Tail End" in "Experimental" tab which is making last tail segments more stiff for some extra tweak for tail motion
- Upgraded behavior of "Gravity" parameter
- Added new example scene and models for testing behavior of Tail Animator for hair

v1.3.5
- Added "Sensitivity" variable to give much more animation customization possibilities for tail motion, this variable making tail being more stiff (when lower) or entangled (when higher)
- Added "Springiness" variable to give much more animation customization possibilities for tail motion, this variable gives tail more jiggly motion when cranked up
- "MaxPositionOffset" variable replaced with "Max Stretching" which is 0-1 slider, on value 1 tail can stretch freely, on value 0 can't stretch a bit, setting this value to about 0.15 is giving the most natural feeling to the motion
- Better support for single bone tail chains (root to parent toggle)
- Cleaning code resulting in few optimization, preparations before next versions optimizations
- New approaches to collisions, now there are three different methods of detecting collisions to choose (only in world space), old one is called "Rotation Offset" (needs bigger sphere tail colliders but works better in collision with mesh colliders), different methods can work better depending on situation etc.
- Now you can choose to use world collision detection (collision space variable) or just include needed colliders without use of rigidbodies (quicker and smoother)
- "Collision Swapping" (under Physics Tab - not available for 'Parental' look up method - it does it partially anyway) which is making collisions being less reflective in segment rotation (will be polished in next updates)
- Added new demo scene "S_TailAnimator_Demo_CollisionWall"
- Added info in ReadMe.txt which directories you can remove to purge unnecessary files if you don't need demo scenes anyomore
- Some cleaning and changes inside inspector window for quicker navigation
+ (hidden update) Different variations of tail model examples models and scene


v1.3.0
- SmoothDelta changed and upgraded to SafeDelta to support more smoothed motion when fps are very unstable
- Added option "Selective Rotations Not Animated" if some bones in tail chain don't have rotation keyframes in source animation
- 'Rolled Bones' replaced with field where we can select LookUp algorithm, added 'CrossUp' algorithm which can support tail motion more precisely than previous methods and 'Parental' which seems to be the most universal but giving a bit different motion (then crank down positions and rotations speeds variable)
- Added 'MaxPositionOffset' variable to limit stretching of tail when object moves very fast
- Added 'Curving' and changed logics of 'Gravity' variable in "Experimental" tab
- Added 'Fuman Elasticness' example scene
- Some upgrades in the inspector window


v1.2.9
- Updated Skinner component to work with newest unity versions
- Updated algorithm for "Rolled Bones" option
- Updated algorithm for 'RootToParent' feature


v1.2.8
- "Advanced" option for waving animation, waving which is using perlin noise to calculate rotation of optional root animation
- Some fixes for inspector windows
- Added icon on the right inside hierarchy to easily indicate objects with attached tail animator
- Added menu items under "Add Component" > "FImpossible Creations" > "Tail Animator" > Tail Animator Components


v1.2.7
- Support for realtime scalling object with tail animator attached


V1.2.6
- Added experimental collision detection feature
- New example scene with collision feature
- Added parameter "Gravity Power" simulating weight of tail
- Some fixes inside editor stuff


V1.2.5
- Added support for one and two - length bone chains
- Added variable "Root To Parent" to make first bone in chain be affected by parent transform (sometimes you will have to toggle "Full Correction" variable to make it work)


V1.2.4
- Added component "FTail_Animator_MassUpdater" which handles tail animators update method in one Update tick, it can boost performance of tail animator when you are using a lot of tail animators (from more than 100 it can give some boost, but when more tail animators, then difference more noticable)
- Added new example scenes:
	Fockatrice: Quadroped creature with tail, wings, long neck and feather like elements, all of that enchanced by tail animator to give you some ideas
	Flime: Not animated slime model with some bones, animated only by tail animator components
	Hair performance tests: Scene with different hairstyles using lots of tail animator components !read provided info on canvases!
	Furry Fiped: Rouch example showing how many tails you can compute in the same time with low cpu overload in reference to components count


V1.2.3
- Added toggle "Queue To Last Update" which is putting component update order to be last, helpful for integrating other components which affecting bones hierarchy from code like Spine Animator
So when you have this option toggles, tail animator will work on bones positions and rotations dictated by spine animator, not by unity animator
- Few small polishes


V1.2.2
- Small tweaks for inspector window
- Added Button "Connect with animator" which is changing variables 'Refresh Helpers', 'Full Correction' and 'Animate Corrections' so you switch to newest feature with one click and more intuitivity
- Added toggle 'Refresh Helpers' which is refreshing helper variables in next frame, to use when your model's T-pose is much different from animations of tail chain you want to animate (for example arms)
this option allows you to add tail animator to character's arms, pelvis etc. enable 'Full Correction' and 'Animate Corrections' so your model starts to be elastic and you can adjust stiffness
- Added manual pdf file with visual friendly description to help you use Tail Animator features in most effetive way


V1.2.1
- Option "Auto go through all bones" under "Tuning Parameters" is renamed to "Full correction" and is upgraded 
to calculate correction for each bone individually, makes it match initial pose at lazy state 
also added new option "Animate Corrections" when you toggle "Full correction" which is matching keyframed animation's rotations


V1.2.0 [Big Update]
- Removed some scripts because they are not needed anymore, they're replaced by more efficient ones
(FTail_FixedUpdate etc. because now you can choose which update clock should be used inside inspector)
- Animator components are renamed to be more intuitive
(FTail_Sinus to FTail_Animator, FTail_MecanimBones to FTail_AnimatorBlending, FTail_2D to FTail_Animator2D etc.)
- Upgraded custom inspector and added rendering gizmos in scene view
- Added icons for individual components so you find them easier
- Added FTail_Editor_Skinner component to skin static meshes onto skinned mesh renderers with tail bone structure inside unity editor
- Now bones hierarchy will not be changed at all in order to animate tail


V1.1.2
- Added "Automatic" tuning option for fixing orientation axes automatically by default
- Added another auto-tuning wrong rotations options, making Tail Animator more universal to cooperate with different skeleton structures


V1.1.0
- Added new examples and components to animate transforms with tail animator in 2D space- UI and Sprites
- Now you can put one transform to bones list and it will be root bone, so you can have component attached to much other object than tail bone
- Custom inspector to see all parameters more clear
- New example scenes
- School of fish example scene


V1.0.1
- Added overrides for Start() method because in some cases it wasn't executed for some reason, probably different .net targets
- Updated fSimpleAssets resources to V1.1


V1.0 - 18/07/2018:
Initial release