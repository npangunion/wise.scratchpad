#include <pch.hpp>
#include <catch.hpp>
#include <spdlog/spdlog.h>
#include <iostream>


template <typename T>
void print(T arg) {
	std::cout << arg << std::endl;
}

template <typename T, typename... Types>
void print(T arg, Types... args) {
	std::cout << arg << ", ";
	print(args...);
}

template <typename... Ints>
int sum_all(Ints... nums) {
	return (... + nums);
}

TEST_CASE("fmt method", "learnfmt")
{

	SECTION("debug")
	{
		std::string s = fmt::format("Number: {}", 3);

		// ����� ������� ����. 
		// - string_view�� ��ƿ. c++17���� ǥ��
		// - vformat, format_to�� ���� ����ȴ�. 
		// - arg�� ������ ���� Ÿ�� �߷����� �� ���� ������ �����Ѵ�. 
		// - �������� ���ڿ� �Ľ̰� �� ��ü�̴�. 
		// - format_arg_store�� ���� ����
		//   - ���Ⱑ MPL�� �ٽ��̴�. 
		// fmt�� std::format()���� c++20���� ǥ��ȭ ������ ��� �ִ�. 
	}

	SECTION("method1. variadic template")
	{
		print(1, 2, 3);

		// print(1, ...args)
		// print(2, ...args)
		// print(3)
		// ��Ϳ� base case
	}

	SECTION("method2. folding")
	{
		// 
		// 1 + 4 + 2 + 3 + 10
		std::cout << sum_all(1, 4, 2, 3, 10) << std::endl;
	}


}
