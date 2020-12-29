#pragma once

#include "garbage_collector.h"
#include "shader_manager.h"
#include "obj_manager.h"
#include "world.h"

#include <string>


struct Mesh : public Garbage
{
	Entity* parent;

	Mesh(Entity* parent);
	void Tick(double dt, ShaderManager* shaderManager);

protected:
	virtual void Draw(ShaderManager* shaderManager) = 0;
};


class SimpleMesh : public Mesh
{
	ShaderManager::ShaderType type = ShaderManager::SIMPLE;

	GLuint attribVertex;
	GLuint attribVNormal;
	GLuint attribTexture;

	GLuint unifTexmap;
	GLuint unifTexmap2;
	GLuint unifColor;

	GLuint unifTexmapK;
	GLuint unifTexmap2K;
	GLuint unifColorK;

	GLfloat texmapK;
	GLfloat texmap2K;
	GLfloat colorK;

	ObjectModel* model;
public:
	SimpleMesh(Entity* parent, ObjectModel* model, ShaderManager* shaderManager,
		GLfloat texmapK, GLfloat texmap2K, GLfloat colorK, ShaderManager::ShaderType type);

protected:
	void Draw(ShaderManager* shaderManager) override;
};


class Graphics : public GarbageCollector<Mesh>
{
	ShaderManager*	shaderManager;
	ObjManager		objManager;

public:
	Graphics(ShaderManager* shaderManager);
	void Tick(double dt);
	SimpleMesh* AddSimpleTextureMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type = ShaderManager::SIMPLE);
	SimpleMesh* AddSimpleColorMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type = ShaderManager::SIMPLE);
	SimpleMesh* AddSimpleColorTextureMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type = ShaderManager::SIMPLE);
	SimpleMesh* AddSimpleTextureTextureMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type = ShaderManager::SIMPLE);
	SimpleMesh* AddSimpleTexture2Mesh(Entity* parent, std::string filename, ShaderManager::ShaderType type = ShaderManager::SIMPLE);
};
