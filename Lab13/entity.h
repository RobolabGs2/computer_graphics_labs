#pragma once

#include "geometry.h"
#include "world.h"
#include "tools.h"

#include <GL\glew.h>
#include <algorithm>
#include <iterator>
#include <string>


#include "helper.h"

class World;

class Entity
{
	struct LightUnif
	{
		GLuint on, pos;
	};
	GLuint lcount;
	std::vector<LightUnif> lights;
public:
	Matrix position = Matrix::Ident();
	Matrix rotation = Matrix::Ident();
	Matrix scale = Matrix::Ident();
	World& world;

	Entity(World& world) :
		world(world)
	{
	}

	Matrix Translation()
	{
		return scale * rotation * position;
	}

	void Move(Point p)
	{
		position = position * Matrix::Move(p);
	}

	void Scale(float scale)
	{
		this->scale = Matrix::Scale(scale, scale, scale);
	}

	void Rotate(Matrix r)
	{
		rotation = rotation * r;
	}

	virtual void Draw()
	{
	};

	virtual ~Entity()
	{
	}

protected:
	void InitLight(GLuint program);

	void SetLight();
};

template <typename T>
class CompositeEntity : public Entity
{
	std::vector<T*> entities;
public:
	CompositeEntity(World& world, const std::string filename): Entity(world)
	{
		auto meshes = TriangleMesh(filename).ToMesh();
		std::transform(meshes.begin(), meshes.end(), std::back_inserter(entities),
		               [&](const Mesh& mesh) { return new T(world, mesh); });
	}

	CompositeEntity(World& world, const std::vector<Mesh> meshes) : Entity(world)
	{
		std::transform(meshes.begin(), meshes.end(), std::back_inserter(entities),
		               [&](Mesh mesh) { return new T(world, mesh); });
	}

	void Draw() override
	{
		for (T* entity : entities)
		{
			entity->Draw(Translation());
		}
	}

	~CompositeEntity()
	{
		for (Entity* e : entities)
			delete e;
	}
};

class TriangleEntity : public Entity
{
	GLuint vertexVbo;
	GLuint normalVbo;
	GLuint program;
	GLuint attribVertex;
	GLuint attribNormal;
	GLuint unifColor;
	GLuint unifTransform;
	GLuint unifCamera;
	GLuint unifProjection;

	Mesh mesh;

	void InitShaders();
	void InitMesh();
	void FreeShaders();
	void FreeMesh();
public:
	TriangleEntity(World& world, const Mesh& mesh);
	void Draw(const Matrix& translation);
	~TriangleEntity() override;
};

class TexturedEntity : public Entity
{
	GLuint vertexVbo;
	GLuint normalVbo;
	GLuint textureVbo;
	GLuint program;
	GLuint attribVertex;
	GLuint attribVColor;
	GLuint attribTexture;
	GLuint unifTransform;
	GLuint unifCamera;
	GLuint unifProjection;
	GLuint unifTexmap;

	Mesh mesh;

	void InitShaders();
	void InitMesh();
	void FreeShaders();
	void FreeMesh();
public:
	TexturedEntity(World& world, const Mesh& mesh);
	void Draw(const Matrix& translation);
	~TexturedEntity() override;
};


class ColorTexturedEntity : public Entity
{
	GLuint vertexVbo;
	GLuint colorVbo;
	GLuint normalVbo;
	GLuint textureVbo;
	GLuint program;
	GLuint attribVertex;
	GLuint attribVColor;
	GLuint attribNormal;
	GLuint attribTexture;
	GLuint unifTransform;
	GLuint unifCamera;
	GLuint unifProjection;
	GLuint unifTexmap;

	Mesh mesh;

	void InitShaders();
	void InitMesh();
	void FreeShaders();
	void FreeMesh();
	Color VertexColor(Point p);
public:
	ColorTexturedEntity(World& world, const Mesh& mesh);
	void Draw(const Matrix& translation);
	~ColorTexturedEntity() override;
};


class TexturedTexturedEntity : public Entity
{
	GLuint vertexVbo;
	GLuint normalVbo;
	GLuint textureVbo;
	GLuint program;
	GLuint attribVertex;
	GLuint attribVColor;
	GLuint attribTexture;
	GLuint unifTransform;
	GLuint unifCamera;
	GLuint unifProjection;
	GLuint unifIntensity;
	GLuint unifTexmap;
	GLuint unifMixtex;

	Mesh mesh;

	void InitShaders();
	void InitMesh();
	void FreeShaders();
	void FreeMesh();
public:
	GLfloat intensity = 0.7;
	TexturedTexturedEntity(World& world, const Mesh& mesh);
	void Draw(const Matrix& translation);
	~TexturedTexturedEntity() override;
};
