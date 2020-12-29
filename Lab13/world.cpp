#include "world.h"

#include <GL\glew.h>
#include <GL\freeglut.h>

World::World()
{ }

void World::Add(Entity* entity)
{
	entities.push_back(entity);
}

void World::Draw()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glClearDepth(1.0f);
	glEnable(GL_DEPTH_TEST);

	for (Entity* e : entities)
		e->Draw();

	glFlush();
	glutSwapBuffers();
}

Matrix World::Camera()
{
	return location* rotation;
}

Matrix World::Projection()
{
	return projection * Matrix::Scale(1, aspectRatio, 1);
}

World::~World()
{
	for (Entity* e : entities)
		delete e;
}
