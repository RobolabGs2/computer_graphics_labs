#pragma once

#include "garbage_collector.h"
#include "world.h"

 
//		$$$$$$$$\  $$$$$$\  $$$$$$$\   $$$$$$\  
//		\__$$  __|$$  __$$\ $$  __$$\ $$  __$$\ 
//		   $$ |   $$ /  $$ |$$ |  $$ |$$ /  $$ |
//		   $$ |   $$ |  $$ |$$ |  $$ |$$ |  $$ |
//		   $$ |   $$ |  $$ |$$ |  $$ |$$ |  $$ |
//		   $$ |   $$ |  $$ |$$ |  $$ |$$ |  $$ |
//		   $$ |    $$$$$$  |$$$$$$$  | $$$$$$  |
//		   \__|    \______/ \_______/  \______/ 


struct Body : Garbage
{
	Entity* parent;
	Body(Entity* parent);
	virtual void Tick(double dt);
};

class MovingRight : Body
{
	
};

class Physics : public GarbageCollector<Body>
{
public:
	void Tick(double dt);
};
