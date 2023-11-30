
// This file has only one job, creating the and starting the game engine.
// Not that the game engine is not the game, in the same scense that Unreal is not Fortnite or BioSock
// If spesifik things need to be told to the game engine, this is when to do it. 

GameEngine game = new(typeof(MasterMind.SplashScreen));
game.Run();
