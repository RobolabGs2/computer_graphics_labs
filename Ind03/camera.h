#pragma once

#include "world.h"
#include "geometry.h"

class Camera
{
public:
	Entity* parent;
	float aspectRatio = 1;

	Camera(Entity* parent = nullptr);
	virtual Matrix Transform();
	virtual Matrix Projection();
};

