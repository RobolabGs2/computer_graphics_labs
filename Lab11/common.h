#pragma once

#include <string>

static const double PI = 3.1415926535897932;

static std::vector<std::string> Split(std::string input, char c, bool skip_empty = true)
{
	input += c;
	std::vector<std::string> result;
	int last = 0;
	for (int i = 0; i < input.size(); ++i)
		if (input[i] == c)
		{
			if (i == last && skip_empty)
			{
				last = i + 1;
				continue;
			}

			result.push_back(input.substr(last, i - last));
			last = i + 1;
		}
	return result;
}

template <typename T>
int Signum(T val) {
	if (T(0) < val)
		return -1;
	if (T(0) > val)
		return 1;
	return 0;
}