#pragma once

#include "geometry.h"

#include <unordered_map>
#include <string>
#include <gl\glew.h>

class ObjManager;

struct ObjectModel
{
	struct ObjMesh
	{
		int		size;
		GLuint	vertexVbo;
		GLuint	normalVbo;
		GLuint	textureVbo;
	};

	struct ObjMaterial
	{
		Color ambient;
		Color diffuse;
		Color specular;
		Color emission;
		signed char shininess = 0;
		GLuint diffuseMap = 0;
		GLuint ambientMap = 0;

		ObjMaterial(const ObjMaterial& other) = default;
		ObjMaterial(ObjMaterial&& other) = default;

		ObjMaterial(Color ambient = { 1, 1, 1 }, Color diffuse = { 1, 1, 1 }, Color specular = { 0, 0, 0 },
			signed char shininess = 0, Color emission = { 0, 0, 0 });
		ObjMaterial(GLuint diffuseMap);
		GLuint ParseMap(const std::string& filename, ObjManager* objManager);
	};

	struct ModelGroup
	{
		ObjMaterial material;
		ObjMesh mesh;

		ModelGroup(const ObjectModel::ObjMaterial& material, const ObjectModel::ObjMesh& mesh);
	};

	std::vector<ModelGroup> models;
};


class ObjManager
{
	std::unordered_map<std::string, GLuint>	textures;
	std::unordered_map<std::string, ObjectModel> models;
public:
	ObjManager() = default;

	ObjManager(const ObjManager&) = delete;
	ObjManager(ObjManager&&) = delete;
	ObjManager& operator=(const ObjManager&) = delete;
	ObjManager& operator=(ObjManager&&) = delete;
	
	ObjectModel* GetModel(const std::string& filename);
	GLuint GetTexture(const std::string& filename);
	
	~ObjManager();
};
