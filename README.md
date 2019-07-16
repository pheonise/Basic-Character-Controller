# Basic Character Controller
This is a basic character controller that uses Rigidbody physics to handle movement. This means easy collisions, and better physical interactions with other physics-based objects.

![image](https://repository-images.githubusercontent.com/194247757/17a52880-99de-11e9-89b4-c414464441d9)

Basic character has movement, jumping, ground detection, and actions. There are a couple of example classes that extend Character with different actions.

Characters are controlled via a Controller. There is both a Player Controller (which uses player input such as keyboard and mouse), and a basic AI Controller (which pathfinds to a random point every few seconds).

Simple Interactions are included, with a Door example in the scene. A Door Handle will open it's attached Door when interacted with, and a Doorbell will play a doorbell sound effect when interacted with.
There are also interactive balls in the scene, which will change to a random color when interacted with. These can also be 'kicked' by the Kicker Character.

This project can be used simply as a learning resource, or as the basis for a game. If you like it or it helps you out, let me know at https://twitter.com/Pheonise
