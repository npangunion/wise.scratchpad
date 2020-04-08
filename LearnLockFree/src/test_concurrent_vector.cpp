#include <pch.hpp>
#include <catch.hpp>

#include "spinlock.hpp"

#include <chrono>
#include <concurrent_vector.h>
#include <mutex>
#include <thread>
#include <unordered_map>


namespace {

struct pc
{
	int id = 0; 
	std::size_t index = 0;

	pc(int _id)
		: id(_id)
	{
	}

};

using vec = Concurrency::concurrent_vector<std::atomic<uintptr_t>>;


class sector
{
	using pc_map = std::unordered_map<int, pc*>;
	
public: 

	void insert(pc* _pc)
	{
		std::lock_guard<util::spinlock> lock(lock_);
		
		pcs_.insert(pc_map::value_type(_pc->id, _pc));

		auto upc = reinterpret_cast<uintptr_t>(_pc);

		for (std::size_t i = 0; i < cv_.size(); ++i)
		{
			if (cv_[i].load() == 0)
			{
				cv_[i] = upc;
				_pc->index = i;
				return;
			}
		}

		cv_.resize(cv_.size() + 1);
		cv_[cv_.size() - 1].exchange(upc);
	}

	void remove(pc* _pc)
	{
		std::lock_guard<util::spinlock> lock(lock_);
		pcs_.erase(_pc->id);
		cv_[_pc->index].exchange(0);

		delete _pc;
	}

	const vec& get_cv() const
	{
		return cv_;
	}

private: 
	util::spinlock lock_;
	pc_map pcs_;
	vec cv_;
};

void sleep(std::size_t msec)
{	
	std::this_thread::sleep_for(std::chrono::milliseconds(msec));
}


} // noname

TEST_CASE("concurrent vector", "sync")
{
	SECTION("basic")
	{
		sector s1;
		std::thread m;
		std::thread s;
		bool stop = false;

		for (int i = 0; i < 200; ++i)
		{
			s1.insert(new pc(i + 1));
		}

		std::thread([&s1, &stop]() 
			{
				while (!stop) {
					pc* pc1 = new pc(500);
					s1.insert(pc1);
					s1.remove(pc1);
				}
			}).swap(m);

		std::thread([&s1, &stop]() 
			{
				while (!stop) 
				{
					auto& cv = s1.get_cv();

					const auto pc2 = reinterpret_cast<pc*>(cv[0].load());
					CHECK(pc2->id == 1);
				
				}
			}).swap(s);

		sleep(10000);

		stop = true;

		m.join();
		s.join();

		// 생각보다 더 어려운 문제이다. 
		// 락프리를 제대로 보겠군. 되면 좋겠다. 
	}
}
