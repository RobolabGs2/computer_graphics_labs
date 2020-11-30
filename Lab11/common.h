#pragma once

#include <string>

static const double PI = 3.1415926535897932;

static std::vector<std::string> Split(std::string input, const std::string& chars, bool skip_empty = true)
{
	input += chars[0];
	std::vector<std::string> result;
	size_t last = 0;
	for (size_t i = 0; i < input.size(); ++i)
		if (chars.find(input[i]) != std::string::npos)
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

static std::vector<std::string> Split(std::string input, char c, bool skip_empty = true)
{
	return Split(input, std::string(1, c), skip_empty);
}

template <typename T>
int Signum(T val)
{
	if (T(0) < val)
		return -1;
	if (T(0) > val)
		return 1;
	return 0;
}
