#include "illumination.h"

#include <Windows.h>
#include <gl\freeglut.h>


//	*****************************************  //
//	**           Illumination              **  //
//	*****************************************  //

void Illumination::Tick(double dt)
{
	GarbageCollector::Tick();
	stupudCounter = 0;
	for (Light* l : data)
		l->Tick(dt);
}

PointLight* Illumination::AddPoint(Entity* parent, Color color, bool enable)
{
	PointLight* result = new PointLight(*this, parent, color, enable);
	GarbageCollector::AddTracking(result);
	return result;
}

DirectionLight* Illumination::AddDirection(Entity* parent, Color color, bool enable)
{
	DirectionLight* result = new DirectionLight(*this, parent, color, enable);
	GarbageCollector::AddTracking(result);
	return result;
}

SpotLight* Illumination::AddSpot(Entity* parent, Color color, float cutoff, bool enable)
{
	SpotLight* result = new SpotLight(*this, parent, color, cutoff, enable);
	GarbageCollector::AddTracking(result);
	return result;
}

GLenum Illumination::GetStupidNumber()
{
	if(stupudCounter == 8)
		throw "stupid GL is died!!";
	return stupidArray[stupudCounter++];
}

//	*****************************************  //
//	**               Light                 **  //
//	*****************************************  //

Light::Light(Illumination& illumination, Entity* parent, Color color, bool enable):
	illumination(illumination),
	parent(parent),
	color(color),
	enable(enable)
{ }

void Light::Tick(double dt)
{
	if (!(alive = parent->alive) || !enable)
		return;
	glPushMatrix();
	parent->TransformGL();
	Draw();
	glPopMatrix();
}


//	*****************************************  //
//	**            PointLight               **  //
//	*****************************************  //

PointLight::PointLight(Illumination& illumination, Entity* parent, Color color, bool enable) :
	Light(illumination, parent, color, enable)
{ }

void PointLight::Draw()
{
	GLenum id = illumination.GetStupidNumber();
	glEnable(id);

	GLfloat direction[] = { 0.0, 0.0, 1.0, 1.0 };
	glLightfv(id, GL_POSITION, direction);

	glLightf(id, GL_LINEAR_ATTENUATION, 0.1);
	color.CallGL([=](auto v) { glLightfv(id, GL_DIFFUSE, v); });
	(color * 0.1).CallGL([=](auto v) { glLightfv(id, GL_AMBIENT, v); });
	Color{ 1, 1, 1 }.CallGL([=](auto v) { glLightfv(id, GL_SPECULAR, v); });
}


//	*****************************************  //
//	**          DirectionLight             **  //
//	*****************************************  //

DirectionLight::DirectionLight(Illumination& illumination, Entity* parent, Color color, bool enable) :
	Light(illumination, parent, color, enable)
{ }

void DirectionLight::Draw()
{
	GLenum id = illumination.GetStupidNumber();
	glEnable(id);

	GLfloat direction[] = { 0.0, 0.0, 1.0, 0.0 };
	glLightfv(id, GL_POSITION, direction);

	color.CallGL([=](auto v) { glLightfv(id, GL_DIFFUSE, v); });
	(color * 0.1).CallGL([=](auto v) { glLightfv(id, GL_AMBIENT, v); });
	Color{ 1, 1, 1 }.CallGL([=](auto v) { glLightfv(id, GL_SPECULAR, v); });
}


//	*****************************************  //
//	**             SpotLight               **  //
//	*****************************************  //

SpotLight::SpotLight(Illumination& illumination, Entity* parent, Color color, float cutoff, bool enable) :
	Light(illumination, parent, color, enable),
	cutoff(cutoff)
{ }

void SpotLight::Draw()
{
	GLenum id = illumination.GetStupidNumber();
	glEnable(id);

	GLfloat direction[] = { 0.0, 0.0, 1.0, 1.0 };
	GLfloat sdirection[] = { 0.0, 0.0, -1.0 };
	glLightfv(id, GL_POSITION, direction);


	glLightf(id, GL_LINEAR_ATTENUATION, 0.1);
	glLightf(id, GL_SPOT_EXPONENT, 15.0);
	glLightf(id, GL_SPOT_CUTOFF, cutoff);
	glLightfv(id, GL_SPOT_DIRECTION, sdirection);
	color.CallGL([=](auto v) { glLightfv(id, GL_DIFFUSE, v); });
	(color * 0.1).CallGL([=](auto v) { glLightfv(id, GL_AMBIENT, v); });
	Color{ 1, 1, 1 }.CallGL([=](auto v) { glLightfv(id, GL_SPECULAR, v); });
}
