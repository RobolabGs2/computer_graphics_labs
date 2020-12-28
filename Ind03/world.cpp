#include "world.h"

#include <Windows.h>
#include <GL\glew.h>
#include <gl\freeglut.h>


//	*****************************************  //
//	**               World                 **  //
//	*****************************************  //

void World::Tick(double dt)
{
	GarbageCollector::Tick();
}

Entity* World::AddEntity(Point location)
{
	Entity* result = new Entity(location);
	GarbageCollector::AddTracking(result);
	return result;
}

TailEntity* World::AddTailEntity(Entity* parent, Point location)
{
	TailEntity* result = new TailEntity(parent, location);
	GarbageCollector::AddTracking(result);
	return result;
}


//	*****************************************  //
//	**               Entity                **  //
//	*****************************************  //

Entity::Entity(Point location) :
	location(location)
{ }

Matrix Entity::Transform()
{
	return
		Matrix::ZRotation(zAngle * PI / 180) *
		Matrix::YRotation(yAngle * PI / 180) *
		Matrix::XRotation(xAngle * PI / 180) *
		Matrix::Move(location);
}

Matrix Entity::ReTransform()
{
	return
		Matrix::Move(-location) *
		Matrix::XRotation(-xAngle * PI / 180) *
		Matrix::YRotation(-yAngle * PI / 180) *
		Matrix::ZRotation(-zAngle * PI / 180);
}


//	*****************************************  //
//	**            TailEntity               **  //
//	*****************************************  //

TailEntity::TailEntity(Entity* parent, Point location) :
	Entity(location),
	parent(parent)
{ }

Matrix TailEntity::Transform()
{
	if (!(alive = parent->alive))
		return Matrix::Move(9999, 9999, 6666);
	return
		Entity::Transform() *
		parent->Transform();
}

Matrix TailEntity::ReTransform()
{
	if (!(alive = parent->alive))
		return Matrix::Ident();
	return
		parent->ReTransform() *
		Entity::ReTransform();
}
