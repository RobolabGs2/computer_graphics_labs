#pragma once

#include "geometry.h"
#include "world.h"
#include "tools.h"

#include <GL\glew.h>

class World;

class Entity
{
public:
	Matrix position = Matrix::Ident();
	Matrix rotation = Matrix::Ident();
	World& world;

	Entity(World& world) :
		world(world)
	{ }

	Matrix Translation()
	{
		return rotation * position;
	}

	void Move(Point p)
	{
		position = position * Matrix::Move(p);
	}

	void Rotate(Matrix r)
	{
		rotation = rotation * r;
	}

	virtual void Draw() = 0;

	virtual ~Entity()
	{ }
};

class StrangeCube : public Entity
{
	GLuint vertexVbo;
	GLuint normalVbo;
	GLuint program;
	GLuint attribVertex;
	GLuint attribVColor;
	GLuint unifTransform;
	GLuint unifCamera;
	GLuint unifProjection;

	void InitShaders();
	void InitMesh();
	void FreeShaders();
	void FreeMesh();
public:
	StrangeCube(World& world);
	void Draw() override;
	~StrangeCube() override;
};

class TriangleEntity : public Entity
{
	GLuint vertexVbo;
	GLuint normalVbo;
	GLuint program;
	GLuint attribVertex;
	GLuint attribVColor;
	GLuint unifTransform;
	GLuint unifCamera;
	GLuint unifProjection;

	Mesh mesh;

	void InitShaders();
	void InitMesh();
	void FreeShaders();
	void FreeMesh();
public:
	TriangleEntity(World& world, const std::string filename);
	void Draw() override;
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
	TexturedEntity(World& world, const std::string filename);
	void Draw() override;
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
	ColorTexturedEntity(World& world, const std::string filename);
	void Draw() override;
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
	TexturedTexturedEntity(World& world, const std::string filename);
	void Draw() override;
	~TexturedTexturedEntity() override;
};

class TexturedBlinnEntity : public Entity
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
	TexturedBlinnEntity(World& world, const std::string filename);
	void Draw() override;
	~TexturedBlinnEntity() override;
};

class TexturedToonEntity : public Entity
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
	TexturedToonEntity(World& world, const std::string filename);
	void Draw() override;
	~TexturedToonEntity() override;
};

class TexturedOtherEntity : public Entity
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
	TexturedOtherEntity(World& world, const std::string filename);
	void Draw() override;
	~TexturedOtherEntity() override;
};
