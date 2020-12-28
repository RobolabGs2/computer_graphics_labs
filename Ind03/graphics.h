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
	const ShaderManager::ShaderType type = ShaderManager::SIMPLE;

	GLuint attribVertex;
	GLuint attribVNormal;
	GLuint attribTexture;
	GLuint unifTexmap;

	ObjectModel* model;
public:
	SimpleMesh(Entity* parent, ObjectModel* model, ShaderManager* shaderManager);

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
	SimpleMesh* AddSimpleMesh(Entity* parent, std::string filename);
};
