#pragma once

#include "camera.h"
#include "world.h"
#include "controller.h"
#include "graphics.h"
#include "physics.h"


class Game
{
public:
	Camera		camera;

	Graphics	graphics;
	Controller	controller;
	Physics		physics;
	World		world;

	struct {
		bool left = false;
		bool right = false;
		bool up = false;
		bool down = false;
		bool space = false;
	} keys;

	Game();

	void Tick(double dt);
	void ResizeWindow(int width, int height);

	Entity* AddCar(Point location, float rotation);
	Entity* AddBullet(Point location, float yAngle);
private:
	void Init();
};
