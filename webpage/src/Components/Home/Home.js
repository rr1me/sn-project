import './Home.sass';

const Home = () => {
    return (
        <div className='home'>
            Наш проект представляет вам набор игровых серверов связанных общей тематикой "выживания" <br/>
            У нас каждый сможет найти себе игру и сервер по душе.

            <div className='status'>
                Статус Серверов <br/>
                {/*1. mine <br/>*/}
                {/*2. 7days <br/>*/}
                {/*3. pz <br/>*/}
                1. Minecraft 		Игроков:  из <br/>
                2. 7Days To Die Игроков 0 из 0, сервер выключен <br/>
                3. Project Zomboid Игроков 0 из 0, сервер выключен <br/>
            </div>
        </div>
    )
};

export default Home;