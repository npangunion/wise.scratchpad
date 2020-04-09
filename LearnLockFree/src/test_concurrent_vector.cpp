#include <pch.hpp>
#include <catch.hpp>

#include "spinlock.hpp"
#include "tick.hpp"

#include <atomic>
#include <chrono>
#include <concurrent_vector.h>
#include <vector>
#include <mutex>
#include <shared_mutex>
#include <thread>
#include <unordered_map>


namespace {

struct pc
{
	int id = 0; 
	std::size_t index = 0;
	bool removed = false;

	pc(int _id)
		: id(_id)
	{
	}

};


class sector
{
	using pc_map = std::unordered_map<int, pc*>;
	using vec = std::vector<pc*>;
	
public: 

	void insert(pc* _pc)
	{
		// pc locked 
		{
			std::lock_guard<util::spinlock> lock(lock_pc_);
			pcs_.insert(pc_map::value_type(_pc->id, _pc));
			_pc->removed = false;
		}

		// sync locked
		{
			std::unique_lock ul(lock_sync_);
			cv_.push_back(_pc);
		}
	}

	void remove(pc* _pc)
	{
		// pc locked
		{
			std::lock_guard<util::spinlock> lock(lock_pc_);
			_pc->removed = true;
			pcs_.erase(_pc->id);
		}

		purge();
	}

	void copy_sync_list(vec& dst)
	{
		std::shared_lock sl(lock_sync_);
		dst.assign(cv_.begin(), cv_.end()); // 전체 복사. copy가 더 빠른가?
	}

private:
	void purge()
	{
		// 더 빠를까? 벡터 복사 부담이 증가하므로 아닐 수도 있다. 
		// 조절이 필요한 부분이다. 

		// 커질 경우 일찍 purge 한다. 
		// 아닐 경우 주기적으로 purge 한다.  

		if (purge_tick_.elapsed() > 1000)
		{
			purge_tick_.reset();

			std::unique_lock ul(lock_sync_);

			cv_.erase(std::find_if(cv_.begin(), cv_.end(),
				[](pc* _pc) {
					return _pc->removed;
				}), cv_.end());
		}
	}

private: 
	util::spinlock lock_pc_;
	std::shared_mutex lock_sync_; // read write lock
	pc_map pcs_;
	vec cv_;
	util::simple_tick purge_tick_;
};

void sleep(std::size_t msec)
{	
	std::this_thread::sleep_for(std::chrono::milliseconds(msec));
}

} // noname

TEST_CASE("concurrent vector", "sync")
{

	SECTION("v2")
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

		std::vector<pc*> lst;

		std::thread([&s1, &stop, &lst]() 
			{
				while (!stop) 
				{
					lst.clear();
					s1.copy_sync_list(lst);

					for (auto& p : lst)
					{
						if (!p->removed)
						{

						}
					}
				}
			}).swap(s);

		sleep(10000);

		stop = true;

		m.join();
		s.join();

		// 생각보다 더 어려운 문제이다. 
		// 락프리를 제대로 보겠군. 되면 좋겠다. 


		//
		// facebook의 folly에 RWSpinlock이 있다. 
		// 
	}
}
