#pragma once

#include "garbage_collector.h"
#include "world.h"

#include <string>
#include <vector>


struct Mesh : public Garbage
{
	Entity* parent;

	Mesh(Entity* parent);
	void Tick(double dt);

protected:
	virtual void Draw() = 0;
};

class Cube : public Mesh
{
	float size;

public:
	Cube(Entity* parent, float size);

protected:
	void Draw() override;
};

class Sphere : public Mesh
{
	float size;
	double radius;

public:
	Sphere(Entity* parent, double radius);

protected:
	void Draw() override;
};

class Cone : public Mesh
{
	float size;
	double base;
	double height;

public:
	Cone(Entity* parent, double base, double height);

protected:
	void Draw() override;
};


class Torus : public Mesh
{
	float size;
	double innerRadius;
	double outerRadius;

public:
	Torus(Entity* parent, double innerRadius, double outerRadius);

protected:
	void Draw() override;
};


class TriangleMesh : public Mesh
{
	struct Polygon
	{
		int vertex[3];
		int texture[3];
		int normal[3];
	};

	std::vector<Polygon> polygons;
	std::vector<Point> vertexes;
	std::vector<Point> normales;
public:
	TriangleMesh(Entity* parent, std::string filename);

protected:
	void Draw() override;
};


class Graphics: public GarbageCollector<Mesh>
{
public:
	void Tick(double dt);
	Cube* AddCube(Entity* parent, double size);
	Sphere* AddSphere(Entity* parent, double radius);
	Cone* AddCone(Entity* parent, double base, double height);
	Torus* AddTorus(Entity* parent, double innerRadius, double outerRadius);
	TriangleMesh* AddTriangleMesh(Entity* parent, std::string filename);
};
