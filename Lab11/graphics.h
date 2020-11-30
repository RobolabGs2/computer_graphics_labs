#pragma once

#include "garbage_collector.h"
#include "world.h"
#include "material.h"

#include <string>
#include <unordered_map>
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
public:
	float size;
	Material material;

	Cube(Entity* parent, const Material& material, float size);

protected:
	void Draw() override;
};

class Sphere : public Mesh
{
public:
	float size;
	double radius;
	Material material;

	Sphere(Entity* parent, const Material& material, double radius);

protected:
	void Draw() override;
};

class Cone : public Mesh
{
public:
	float size;
	double base;
	double height;
	Material material;

	Cone(Entity* parent, const Material& material, double base, double height);

protected:
	void Draw() override;
};


class Torus : public Mesh
{
	float size;
	double innerRadius;
	double outerRadius;
	Material material;

public:
	Torus(Entity* parent, const Material& material, double innerRadius, double outerRadius);

protected:
	void Draw() override;
};


class Plane : public Mesh
{
	float xSize;
	float zSize;
	int xPartition;
	int zPartition;
	Material material;
	
public:
	Plane(Entity* parent, float xSize, float zSize, int xPartition, int zPartition, Material material);

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

	struct Object
	{
		std::vector<Polygon> polygons;
		std::string mtl;
	};

	std::unordered_map<std::string, Material> mtlLibrary;
	
	std::vector<Object> objects;
	std::vector<Point> vertexes;
	std::vector<Point> normales;
	std::vector<Point> textures;

public:
	TriangleMesh(Entity* parent, std::string filename);

protected:
	void Draw() override;
};


class Graphics: public GarbageCollector<Mesh>
{
public:
	void Tick(double dt);
	Cube* AddCube(Entity* parent, double size, const Material& material = Material::defaultMaterial);
	Sphere* AddSphere(Entity* parent, double radius, const Material& material = Material::defaultMaterial);
	Cone* AddCone(Entity* parent, double base, double height, const Material& material = Material::defaultMaterial);
	Torus* AddTorus(Entity* parent, double innerRadius, double outerRadius, const Material& material = Material::defaultMaterial);
	Plane* AddPlane(Entity* parent, float xSize, float zSize, const Material& material = Material::defaultMaterial, int xPartition = 10, int zPartition = 10);
	TriangleMesh* AddTriangleMesh(Entity* parent, std::string filename);
};
