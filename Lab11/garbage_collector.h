#pragma once

#include <vector>
#include <unordered_set>

class Garbage
{
public:
	bool alive = true;

	virtual ~Garbage()
	{ }
};

template<typename T>
class GarbageCollector
{
	void Clear()
	{
		for (T* element : trash)
			delete element;
		trash.clear();
	}

	GarbageCollector(const GarbageCollector&) = delete;
	GarbageCollector(GarbageCollector&&) = delete;
	GarbageCollector& operator=(const GarbageCollector&) = delete;
	GarbageCollector& operator=(GarbageCollector&&) = delete;

protected:
	std::vector<T*>			trash;
	std::unordered_set<T*>	data;

	void AddTracking(T* element)
	{
		data.insert(element);
	}

	void Tick()
	{
		Clear();
		for (T* element : data)
			if (!element->alive)
				trash.push_back(element);
		for (T* element : trash)
			data.erase(element);
	}
public:
	GarbageCollector()
	{ }


	virtual ~GarbageCollector()
	{
		Clear();
		for(T* element: data)
			delete element;
	}
};
