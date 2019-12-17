![](https://github.com/Tobbse/VR_Project/blob/master/Documentation/nitro_screenshot_150919.PNG)

# VR_Project
*UPDATE:
The whole thing is on hold now as I finished the project. It is not very polished and frankly in a somewhat buggy alpha-state due to time limitations (I already stepped over the required amount of Credit Points by quite a lot), but if you're just looking for some inspiration to create your own BeatSaber clone, this will most likely be helpful. I might look into improving it after I've finished working on my thesis.*

This is a Unity C# VR project developed for my Master's degree in Computer Science at the University of Applied Sciences Wedel.

The main idea was to implement all major features used in Beat Saber and therefore imitating its Core Game Loop as much as possible, limited by the 5 credit points rewarded for it. As a special improvement, an onset detection (or beat detection) algorithm was developed in order to procedurally generate Beat Saber levels based on audio data. The BeatSaber Clone can either create those beat mappings itself or use an existing mapping, e.g. from bsaber.com.

Check out the API documentation here:
https://tobbse.github.io/VR_Project/api/index.html

The documentation can be found here (German only):
https://github.com/Tobbse/VR_Project/blob/master/Documentation/VR_Project_Documentation_GERMAN.pdf

The procedural level generation and audio analysis is based on a paper published by Simon Dixon from the Austrian Research Institute for Artificial Intelligence, "Onset Detection Revisited". It can be found here:
https://www.eecs.qmul.ac.uk/~simond/pub/2006/dafx.pdf
