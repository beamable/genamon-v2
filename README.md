# genamon
Genamon sample project implemented with OpenAI, with optional web3 integration (see other branches).

# Overview
This project showcases the ability to generate collectable creatures with AI, "Genamon", which are inspired from Pokemon. 
The project is powered by Beamable, and will first generate in a Microservice the stats, description and other text data for the genamon.
Once that is complete, it will feed the Genamon description to the DALL-E image model to generate image assets at runtime.
Once OpenAI has completed the generation, Beamable will publish a notification to the client which will "spawn" the Genamon in the world.

You can capture Genamon, which causes the image to be uploaded to the Beamable CDN, and the item to be added to the player's Beamable inventory! 

# Getting Started
You will need:
- An OpenAI api key (https://openai.com/). Make sure that you have enough credits in your OpenAI account!

Once you have obtained the above, visit the Beamable portal, "Config" section, and add:
- Namespace = "game", Key = "openai_key", Value = <your-openai-key>

Running the Microservices
1. Make sure you have completed the OpenAI key configuration in the Portal
2. Open the Beamable Content Manager and click the "Publish" button to deploy content
3. Open the Beamable Microservices Manager and click the "Publish" button to deploy the Microservices to the Cloud

# Web3 Integration
An added benefit of this project is its integration with various web3 projects. Have a look at the other branches in this repo for details.

# Special Thanks
This project was made possible with free assets from the following talented creators:
- Pixel Art Top Down - Basic (https://assetstore.unity.com/packages/2d/environments/pixel-art-top-down-basic-187605)
- Fantasy Wooden GUI (https://assetstore.unity.com/packages/p/fantasy-wooden-gui-free-103811)
- Cartoon FX Remaster (https://assetstore.unity.com/packages/p/cartoon-fx-remaster-free-109565)
- Particle Effects for UGUI (https://github.com/mob-sakai/ParticleEffectForUGUI)
