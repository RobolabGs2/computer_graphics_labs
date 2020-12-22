#pragma once

#include "entity.h"

#include <vector>

class Entity;

class World
{
	std::vector<Entity*> entities;
public:
	Matrix projection = Matrix::Ident();
	Matrix location = Matrix::Ident();
	Matrix rotation = Matrix::Ident();

	World();

	World(World&) = delete;
	World(World&&) = delete;
	World& operator=(World&) = delete;
	World& operator=(World&&) = delete;

	Matrix Camera();
	Matrix Projection();

	void Add(Entity* entity);
	void Draw();
	~World();
};
