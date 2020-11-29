#pragma once

#include "garbage_collector.h"
#include "world.h"
#include "color.h"

#include <Windows.h>
#include <GL/GL.h>

class Illumination;

struct Light : public Garbage
{
	Illumination& illumination;
	Entity* parent;
	Color color;
	bool enable;

	Light(Illumination& illumination, Entity* parent, Color color, bool enable);
	void Tick(double dt);

protected:
	virtual void Draw() = 0;
};

struct PointLight : public Light
{
	PointLight(Illumination& illumination, Entity* parent, Color color, bool enable);

protected:
	void Draw() override;
};

struct DirectionLight : public Light
{
	DirectionLight(Illumination& illumination, Entity* parent, Color color, bool enable);

protected:
	void Draw() override;
};

struct SpotLight : public Light
{
	float cutoff;
	SpotLight(Illumination& illumination, Entity* parent, Color color, float cutoff, bool enable);

protected:
	void Draw() override;
};

class Illumination : public GarbageCollector<Light>
{
	GLenum stupidArray[8] = {
		GL_LIGHT0,
		GL_LIGHT1,
		GL_LIGHT2,
		GL_LIGHT3,
		GL_LIGHT4,
		GL_LIGHT5,
		GL_LIGHT6,
		GL_LIGHT7
	};
	int stupudCounter = 0;

public:
	void Tick(double dt);
	PointLight* AddPoint(Entity* parent, Color color, bool enable = true);
	DirectionLight* AddDirection(Entity* parent, Color color, bool enable = true);
	SpotLight* AddSpot(Entity* parent, Color color, float cutoff, bool enable = true);
	GLenum GetStupidNumber();
};
