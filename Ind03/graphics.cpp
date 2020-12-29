#include "graphics.h"
#include "helper.h"

#include <GL\glew.h>
#include <gl\freeglut.h>
#include <fstream>
#include <iostream>

//	*****************************************  //
//	**             Graphics                **  //
//	*****************************************  //


Graphics::Graphics(ShaderManager* shaderManager):
	shaderManager(shaderManager)
{ }

void Graphics::Tick(double dt)
{
	GarbageCollector::Tick();
	for (Mesh* m : data)
		m->Tick(dt, shaderManager);
}

SimpleMesh* Graphics::AddSimpleTextureMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type)
{
	SimpleMesh* result = new SimpleMesh(parent, objManager.GetModel(filename), shaderManager, 1, 0, 0, type);
	GarbageCollector::AddTracking(result);
	return result;
}

SimpleMesh* Graphics::AddSimpleColorMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type)
{
	SimpleMesh* result = new SimpleMesh(parent, objManager.GetModel(filename), shaderManager, 0, 0, 1, type);
	GarbageCollector::AddTracking(result);
	return result;
}

SimpleMesh* Graphics::AddSimpleColorTextureMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type)
{
	SimpleMesh* result = new SimpleMesh(parent, objManager.GetModel(filename), shaderManager, 0.5, 0, 0.5, type);
	GarbageCollector::AddTracking(result);
	return result;
}

SimpleMesh* Graphics::AddSimpleTextureTextureMesh(Entity* parent, std::string filename, ShaderManager::ShaderType type)
{
	SimpleMesh* result = new SimpleMesh(parent, objManager.GetModel(filename), shaderManager, 0.7, 0.3, 0, type);
	GarbageCollector::AddTracking(result);
	return result;
}

SimpleMesh* Graphics::AddSimpleTexture2Mesh(Entity* parent, std::string filename, ShaderManager::ShaderType type)
{
	SimpleMesh* result = new SimpleMesh(parent, objManager.GetModel(filename), shaderManager, 0, 1, 0, type);
	GarbageCollector::AddTracking(result);
	return result;
}

//	*****************************************  //
//	**               Mesh                  **  //
//	*****************************************  //

Mesh::Mesh(Entity* parent) :
	parent(parent)
{
}

void Mesh::Tick(double dt, ShaderManager* shaderManager)
{
	if (!(alive = parent->alive))
		return;

	shaderManager->SetTransform(parent->Transform());
	Draw(shaderManager);
}


//	*****************************************  //
//	**            SimpleMesh               **  //
//	*****************************************  //

SimpleMesh::SimpleMesh(Entity* parent, ObjectModel* model, ShaderManager* shaderManager,
	GLfloat texmapK, GLfloat texmap2K, GLfloat colorK, ShaderManager::ShaderType type):
	Mesh(parent), model(model), texmapK(texmapK), texmap2K(texmap2K), colorK(colorK), type(type),
	attribVertex(shaderManager->AttribLocation("coord", type)),
	attribVNormal(shaderManager->AttribLocation("normal", type)),
	attribTexture(shaderManager->AttribLocation("texture", type)),
	unifTexmap(shaderManager->UnifLocation("texmap", type)),
	unifTexmap2(shaderManager->UnifLocation("texmap2", type)),
	unifColor(shaderManager->UnifLocation("fill_color", type)),
	unifTexmapK(shaderManager->UnifLocation("texmap_k", type)),
	unifTexmap2K(shaderManager->UnifLocation("texmap2_k", type)),
	unifColorK(shaderManager->UnifLocation("fill_color_k", type))
{ }

void SimpleMesh::Draw(ShaderManager* shaderManager)
{
	shaderManager->UseShader(type);

	for (auto& unit: model->models)
	{
		shaderManager->EnableArrayBuffer(unit.mesh.vertexVbo, attribVertex, 3);
		shaderManager->EnableArrayBuffer(unit.mesh.normalVbo, attribVNormal, 3);
		shaderManager->EnableArrayBuffer(unit.mesh.textureVbo, attribTexture, 2);

		if (texmapK > 0) shaderManager->SetTexture0(unifTexmap, unit.material.diffuseMap);
		if (texmap2K > 0) shaderManager->SetTexture1(unifTexmap2, unit.material.ambientMap);
		if (colorK > 0) shaderManager->SetUniform(unifColor, unit.material.diffuse);

		shaderManager->SetUniform(unifTexmapK, texmapK);
		shaderManager->SetUniform(unifTexmap2K, texmap2K);
		shaderManager->SetUniform(unifColorK, colorK);

		shaderManager->Run(unit.mesh.size);

		shaderManager->DisableArrayBuffer(attribTexture);
		shaderManager->DisableArrayBuffer(attribVNormal);
		shaderManager->DisableArrayBuffer(attribVertex);
		checkOpenGLerror();
	}
}
