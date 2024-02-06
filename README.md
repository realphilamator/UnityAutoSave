<h1>Introduction</h1>

Has this ever happened to you?: You are working on something, then your computer crashes, or unity crashes, etc. well i have just the solution!
I have made a script that work in the unity editor, and doesnt even need a game object! and im ready to release to the public!

What it does:

It creates a file called <YourSceneName>.autosave.<Number>.unity in a folder it creates (or you created) called Autosaves,

if the amount of autosaves exceed 10, then it deletes one so you dont have billions of autosave files.

<h1>How to use</h1>

Import the .cs file into unity, and it'll automatically create a folder called "Autosaves" and a folder of whatever your active scene is

<h3>Hierarchy example</h3>

Assets\

	Autosaves\

		Game\
			Game.autosave.7.unity
		Test\
			Test.autosave.3.unity

<h3>Windows</h3>
There is a window in Tools that allows you to configure the autosave!
