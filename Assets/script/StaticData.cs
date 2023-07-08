using System;

namespace script {
    public static class StaticData {
        public static Action OnGameWin;
        public static Action OnGameLose;

        public static Action OnZombieGain;
        public static Action OnZombieLose;

        public static int ZombieCount;

        public static void ZombieLose() {
            ZombieCount--;
            OnZombieLose?.Invoke();
            if( ZombieCount<=0) OnGameLose?.Invoke();
        }
    }
}