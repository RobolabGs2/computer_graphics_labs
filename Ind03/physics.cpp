#include "physics.h"
#include "geometry.h"

#include <Windows.h>
#include <gl\freeglut.h>
#include <iostream>


static void Intersection(StaticCube* b1, StaticCube* b2);
static void Intersection(DynamicCylinder* b1, StaticCube* b2);
static void Intersection(DynamicCylinder* b1, DynamicCylinder* b2);

template <typename T>
static int Signum(T val);

//	*****************************************  //
//	**              Physics                **  //
//	*****************************************  //

void Physics::Tick(double dt)
{
	GarbageCollector::Tick();
	for (Body* b : data)
		b->Tick(dt);

	int pop_count = 0;
	for (int i = 0; i < sortedBodyes.size(); ++i)
	{
		if (!sortedBodyes[i]->alive)
		{
			++pop_count;
			continue;
		}

		float x1 = sortedBodyes[i]->Abba().minPoint.x;
		for (int j = i - 1; j >= 0; --j)
		{
			float x2 = sortedBodyes[j]->Abba().minPoint.x;
			if (x1 >= x2 && sortedBodyes[j]->alive)
				break;
			std::swap(sortedBodyes[j + 1], sortedBodyes[j]);
		}
	}

	for (int i = 0; i < pop_count; ++i)
		sortedBodyes.pop_back();

	for (int i = 0; i < sortedBodyes.size(); ++i)
	{
		float x1 = sortedBodyes[i]->Abba().maxPoint.x;
		for (int j = i + 1; j < sortedBodyes.size(); ++j) {
			float x2 = sortedBodyes[j]->Abba().minPoint.x;
			if (x2 > x1)
				break;
			sortedBodyes[i]->Hit(sortedBodyes[j]);
		}
	}
}

StaticCube* Physics::AddStaticCube(Entity* parent, Point size)
{
	StaticCube* result = new StaticCube(parent, size);
	GarbageCollector::AddTracking(result);
	sortedBodyes.push_back(result);
	return result;
}

DynamicCylinder* Physics::AddDynamicCylinder(Entity* parent, float radius, float height)
{
	DynamicCylinder* result = new DynamicCylinder(parent, radius, height);
	GarbageCollector::AddTracking(result);
	sortedBodyes.push_back(result);
	return result;
}

//	*****************************************  //
//	**               Body                  **  //
//	*****************************************  //

Body::Body(Entity* parent) :
	parent(parent)
{ }

void Body::Tick(double dt)
{
	if (!(alive = parent->alive))
		return;
}

//	*****************************************  //
//	**            StaticCube               **  //
//	*****************************************  //


StaticCube::StaticCube(Entity* parent, Point size) :
	Body(parent), size(size)
{ }

void StaticCube::Tick(double dt)
{
	Body::Tick(dt);
	if (!alive) return;
}

ABBA StaticCube::Abba()
{
	return {
		parent->location - size / 2,
		parent->location + size / 2
	};
}

void StaticCube::Hit(Body* body)
{
	body->Hit(this);
}

void StaticCube::Hit(StaticCube* body)
{
	Intersection(this, body);
}

void StaticCube::Hit(DynamicCylinder* body)
{
	Intersection(body, this);
}

//	*****************************************  //
//	**           DynamicCylinder           **  //
//	*****************************************  //


DynamicCylinder::DynamicCylinder(Entity* parent, float radius, float height) :
	Body(parent), radius(radius), height(height)
{ }

void DynamicCylinder::Tick(double dt)
{
	Body::Tick(dt);
	if (!alive) return;

	float m = 100;

	float rad_angle = parent->yAngle * 2 * PI / 360;;
	Point rotate_friction = {
		std::abs(friction.x * cos(rad_angle) - friction.z * sin(rad_angle)),
		std::abs(friction.y),
		std::abs(friction.x * sin(rad_angle) + friction.z * cos(rad_angle)),
	};

	velocity = velocity + force * dt * m;
	velocity = {
		velocity.x * cos(-rad_angle) + velocity.z * sin(-rad_angle),
		velocity.y,
		-velocity.x * sin(-rad_angle) + velocity.z * cos(-rad_angle),
	};
	velocity = velocity - velocity * friction;
	velocity = {
		velocity.x * cos(rad_angle) + velocity.z * sin(rad_angle),
		velocity.y,
		-velocity.x * sin(rad_angle) + velocity.z * cos(rad_angle),
	};

	parent->location = parent->location + velocity * dt;
	force = { 0, gravity ? -1.0f : 0.0f, 0 };
}

ABBA DynamicCylinder::Abba()
{
	return {
		{
			parent->location.x - radius,
			parent->location.y - height / 2,
			parent->location.z - radius,
		},
		{
			parent->location.x + radius,
			parent->location.y + height / 2,
			parent->location.z + radius,
		},
	};
}

void DynamicCylinder::Hit(Body* body)
{
	body->Hit(this);
}

void DynamicCylinder::Hit(StaticCube* body)
{
	Intersection(this, body);
}

void DynamicCylinder::Hit(DynamicCylinder* body)
{
	Intersection(this, body);
}


//	*****************************************  //
//	**            Intersection             **  //
//	*****************************************  //

static void Intersection(StaticCube* b1, StaticCube* b2)
{
	//	Ну, на то они и статические, что бы тут ничего не происходило
}

static void Intersection(DynamicCylinder* b1, StaticCube* b2)
{
	if (b1->parent->location.y + b1->height / 2 < b2->parent->location.y - b2->size.y / 2 ||
		b1->parent->location.y - b1->height / 2 > b2->parent->location.y + b2->size.y / 2 ||
		b1->parent->location.x + b1->radius < b2->parent->location.x - b2->size.x / 2 ||
		b1->parent->location.x - b1->radius > b2->parent->location.x + b2->size.x / 2 ||
		b1->parent->location.z + b1->radius < b2->parent->location.z - b2->size.z / 2 ||
		b1->parent->location.z - b1->radius > b2->parent->location.z + b2->size.z / 2)
		return;

	Point delta = b2->parent->location - b1->parent->location;
	Point abs_d = delta.Abs();

	Point collision = Point{
		(b1->radius + b2->size.x / 2) - abs_d.x,
		(b1->height / 2 + b2->size.y / 2) - abs_d.y,
		(b1->radius + b2->size.z / 2) - abs_d.z,
	} / 2;

	float jump = delta.y < 0 ? 0.3 : 0;

	if (collision.y - jump < collision.x && collision.y - jump < collision.z)
	{
		b1->velocity.y = 0;
		b1->parent->location.y += (collision.y + 1e-10) * Signum(delta.y);
	}
	else
		if (collision.x < collision.z)
		{
			b1->velocity.x = 0;
			b1->parent->location.x += (collision.x + 1e-10) * Signum(delta.x);
		}
		else
		{
			b1->velocity.z = 0;
			b1->parent->location.z += (collision.z + 1e-10) * Signum(delta.z);
		}
	b1->lastSCubeCollision = b2;
}

static void Intersection(DynamicCylinder* b1, DynamicCylinder* b2)
{
	if (b1->parent->location.y + b1->height / 2 < b2->parent->location.y - b2->height / 2 ||
		b1->parent->location.y - b1->height / 2 > b2->parent->location.y + b2->height / 2)
		return;
	Point delta = b2->parent->location - b1->parent->location;
	float dist2 = delta.x * delta.x + delta.z * delta.z;
	float rad_sum = b1->radius + b2->radius;
	if (dist2 > rad_sum * rad_sum)
		return;

	Point force = delta.Norm() * std::exp(rad_sum - std::sqrt(dist2));
	b1->force = -force;
	b2->force = force;

	b1->lastDCylinderCollision = b2;
	b2->lastDCylinderCollision = b1;
}

template <typename T>
static int Signum(T val)
{
	if (T(0) < val)
		return -1;
	if (T(0) > val)
		return 1;
	return 0;
}
