#pragma once

#include "camera.h"
#include "world.h"
#include "controller.h"
#include "graphics.h"
#include "physics.h"
#include "illumination.h"
#include <string>

class Game
{
public:
	Camera			camera;
	Entity*			user;
	Bus*			game_bus;
	Tank*			game_tank;
	Gun*			game_gun;
	Entity*			tree;
	
	Graphics		graphics;
	Controller		controller;
	Physics			physics;
	Illumination	illumination;
	World			world;

	bool gamemode = true;

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

	Entity* AddCar(Point location, float rotation);
	Entity* AddTank(Point location);
	Entity* AddGun(Point location);
	Entity* AddBus(Point location);
	Entity* AddTree();
	Entity* AddUserCar(Point location);
	Entity* AddBullet(Point location, float yAngle, float size = 0.2, float speed = 100, float v_speed = 0);
	Entity* AddLantern(Point location, float rotation);
	Entity* AddHouse(Point location);

private:
	void Init();
	enum textures
	{
		Ball,
		Flowers,
		Glass,
		GoldenGlitter,
		GoldenGreen,
		Santa,
		Xmas,
		Pink,
		White,
		Yellow,
		Blue,
		Tree,
		Floor,
	};	
	Material getTexture(textures name);
	
};
