#pragma once

#include "camera.h"
#include "world.h"
#include "graphics.h"
#include "physics.h"
#include "controller.h"
#include "shader_manager.h"
#include <string>

class Game
{
public:
	Camera		camera;
	Entity*		user;
	
	ShaderManager shaderManager;

	Controller	controller;
	Graphics	graphics;
	Physics		physics;
	World		world;

	struct {
		bool left = false;
		bool right = false;
		bool up = false;
		bool down = false;
		bool space = false;
		bool headlamp = false;
		bool lantern = false;
	} keys;


	Game();

	void Tick(double dt);
	void ResizeWindow(int width, int height);
private:
	void Init();
};
