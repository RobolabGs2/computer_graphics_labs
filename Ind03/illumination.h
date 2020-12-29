#pragma once

#include "world.h"
#include "garbage_collector.h"
#include "shader_manager.h"
#include "geometry.h"

class ShaderManager;

struct RawSpotLight
{
	float lx;
	float ly;
	float lz;
	float d;
	float vx;
	float vy;
	float vz;
	float a;
};

struct SpotLight: public Garbage
{
	Entity* parent;
	GLfloat alpha;
	GLfloat range;

	bool	enable = true;

	SpotLight(Entity* parent, GLfloat alpha, GLfloat range);
	void Tick(double dt);
};

class Illumination: public GarbageCollector<SpotLight>
{
	ShaderManager* shaderManager;
public:
	Illumination(ShaderManager* shaderManager);
	void Tick(double dt);
	SpotLight* AddSpotLight(Entity* parent, GLfloat alpha = 180, GLfloat range = 1);
};