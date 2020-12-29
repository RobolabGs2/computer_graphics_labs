#pragma once

#include "geometry.h"

#include <vector>
#include <unordered_map>

struct Mesh
{
	std::vector<Point> points;
	std::vector<Point> normals;
	std::vector<Point2> textures;

	Color diffuse;

	GLuint diffuseMap;
	GLuint ambientMap;
};

struct Material
{
	Color ambient;
	Color diffuse;
	Color specular;
	Color emission;
	signed char shininess = 0;
	GLuint diffuseMap = 0;
	GLuint ambientMap = 0;

	Material(Color ambient = { 1, 1, 1 }, Color diffuse = { 1, 1, 1 }, Color specular = { 0, 0, 0 },
		signed char shininess = 0, Color emission = { 0, 0, 0 });
	Material(GLuint diffuseMap);
	GLuint ParseMap(const std::string& diffuseMapFilename);
};


class TriangleMesh
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

	struct Obj
	{
		std::unordered_map<std::string, Material> mtlLibrary;

		std::vector<Object> objects;
		std::vector<Point> vertexes;
		std::vector<Point> normales;
		std::vector<Point2> textures;
	};

	Obj model;
public:
	TriangleMesh(std::string filename);
	std::vector<Mesh> ToMesh();
};
