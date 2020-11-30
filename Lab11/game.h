#pragma once

#include "camera.h"
#include "world.h"
#include "controller.h"
#include "graphics.h"
#include "physics.h"
#include "illumination.h"


class Game
{
public:
	Camera			camera;

	Graphics		graphics;
	Controller		controller;
	Physics			physics;
	Illumination	illumination;
	World			world;

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
	Entity* AddTank(Point location, float rotation);
	Entity* AddGun(Point location, float rotation);
	Entity* AddBus(Point location, float rotation);
	Entity* AddUserCar(Point location);
	Entity* AddBullet(Point location, float yAngle);
	Entity* AddLantern(Point location, float rotation);

private:
	void Init();
};
