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

SimpleMesh* Graphics::AddSimpleMesh(Entity* parent, std::string filename)
{
	SimpleMesh* result = new SimpleMesh(parent, objManager.GetModel(filename), shaderManager);
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

SimpleMesh::SimpleMesh(Entity* parent, ObjectModel* model, ShaderManager* shaderManager):
	Mesh(parent), model(model),
	attribVertex(shaderManager->AttribLocation("coord", type)),
	attribVNormal(shaderManager->AttribLocation("normal", type)),
	attribTexture(shaderManager->AttribLocation("texture", type)),
	unifTexmap(shaderManager->UnifLocation("texmap", type))
{ }

void SimpleMesh::Draw(ShaderManager* shaderManager)
{
	shaderManager->UseShader(type);

	for (auto& unit: model->models)
	{
		shaderManager->EnableArrayBuffer(unit.mesh.vertexVbo, attribVertex, 3);
		shaderManager->EnableArrayBuffer(unit.mesh.normalVbo, attribVNormal, 3);
		shaderManager->EnableArrayBuffer(unit.mesh.textureVbo, attribTexture, 2);
		shaderManager->SetTexture0(unifTexmap, unit.material.diffuseMap);

		shaderManager->Run(unit.mesh.size);

		shaderManager->DisableArrayBuffer(attribTexture);
		shaderManager->DisableArrayBuffer(attribVNormal);
		shaderManager->DisableArrayBuffer(attribVertex);
		checkOpenGLerror();
	}
}
