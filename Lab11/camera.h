#pragma once

#include "point.h"
#include "world.h"

class Camera
{
public:
	Entity* parent;
	float aspectRatio = 1;

	Camera(Entity* parent = nullptr);
	virtual void TransformGL();
};
