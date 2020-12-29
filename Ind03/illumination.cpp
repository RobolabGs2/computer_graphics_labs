#include "illumination.h"

#include <vector>


//	*****************************************  //
//	**           Illumination              **  //
//	*****************************************  //

Illumination::Illumination(ShaderManager* shaderManager):
	shaderManager(shaderManager)
{ }

void Illumination::Tick(double dt)
{
	GarbageCollector::Tick();
	std::vector<RawSpotLight> rawLights(data.size());

	int i = 0;
	for (SpotLight* l : data)
	{
		l->Tick(dt);
		if (!l->enable)
			continue;

		Matrix m =
			l->parent->Transform() *
			shaderManager->cameraMatrix;

		Point location = l->parent->location * m;
		Point direction = Point{ 0, 0, 1.0 } * m - Point{ 0, 0, 0 } * m;
		rawLights[i] = {
			location.x, location.y, location.z, l->range,
			direction.x, direction.y, direction.z, cos(l->alpha * PI / 180)
		};
		++i;
	}
	shaderManager->Initlight(rawLights.data(), i);
}

SpotLight* Illumination::AddSpotLight(Entity* parent, GLfloat alpha, GLfloat range)
{
	SpotLight* result = new SpotLight{ parent, alpha, range };
	GarbageCollector::AddTracking(result);
	return result;
}

//	*****************************************  //
//	**             SpotLight               **  //
//	*****************************************  //


SpotLight::SpotLight(Entity* parent, GLfloat alpha, GLfloat range):
	parent(parent),
	alpha(alpha),
	range(range)
{ }

void SpotLight::Tick(double dt)
{
	alive = parent->alive;
}
