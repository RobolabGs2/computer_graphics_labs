#pragma once
#include <string>

#include "color.h"

struct Material
{
public:
	Color ambient, diffuse, specular, emission;
	signed char shininess = 0;
	GLuint diffuseMap = 0;

	void Activate() const;

	void Deactivate() const;


	Material(Color ambient = {1, 1, 1}, Color diffuse = {1, 1, 1}, Color specular = {0, 0, 0},
	         signed char shininess = 0, Color emission = { 0, 0, 0 });

	Material(GLuint diffuseMap);
	Material(const std::string& diffuseMapFilename);
	static const Material defaultMaterial;
};


