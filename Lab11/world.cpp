#include "world.h"

#include <Windows.h>
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

void Entity::TransformGL()
{
	glTranslated(location.x, location.y, location.z);
	glRotatef(xAngle, 1, 0, 0);
	glRotatef(yAngle, 0, 1, 0);
	glRotatef(zAngle, 0, 0, 1);
}

 void Entity::ReTransformGL()
{
	glRotatef(-zAngle, 0, 0, 1);
	glRotatef(-yAngle, 0, 1, 0);
	glRotatef(-xAngle, 1, 0, 0);
	glTranslated(-location.x, -location.y, -location.z);
}


//	*****************************************  //
//	**            TailEntity               **  //
//	*****************************************  //

TailEntity::TailEntity(Entity* parent, Point location) :
	Entity(location),
	parent(parent)
{ }

void TailEntity::TransformGL()
{
	if (!(alive = parent->alive))
	{
		glTranslated(9999, 9999, 6666);
		return;
	}
	parent->TransformGL();
	Entity::TransformGL();
}

void TailEntity::ReTransformGL()
{
	if (!(alive = parent->alive)) return;
	Entity::ReTransformGL();
	parent->ReTransformGL();
}

