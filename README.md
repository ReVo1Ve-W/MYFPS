# MYFPS — FPS 塔防游戏

Unity 第一人称射击 + 塔防玩法，在城市场景中抵御蜘蛛机器人进攻，保护中央水塔。

## 游戏玩法

- 玩家在城市中自由移动、射击
- 蜘蛛机器人从水塔周围随机刷新，优先攻击水塔
- 玩家靠近敌人时会吸引仇恨
- 每波敌人数量、血量、速度递增
- 水塔被摧毁则游戏结束

## 操作

| 按键 | 功能 |
|---|---|
| WASD | 移动 |
| Shift | 跑步 |
| 空格 | 跳跃 |
| 鼠标移动 | 视角 |
| 右键 | 瞄准 |
| 左键 | 开火 |

## 项目结构

```
Assets/
├── Scripts/                  # C# 脚本
│   ├── PlayerControl.cs      # FPS 控制器（移动/视角/血量）
│   ├── WeaponControl.cs      # 武器开火
│   ├── BulletControl.cs      # 子弹物理与伤害
│   ├── RecoilControl.cs      # 后坐力
│   ├── EnemyAI.cs            # 敌人 AI（Navigate/Attack 双状态）
│   ├── EnemyControl.cs       # 敌人血量与受伤
│   ├── WaveManager.cs        # 波次系统 + 随机刷怪
│   ├── ObjectPool.cs         # 通用对象池
│   ├── DefenseTarget.cs      # 防御目标（水塔）
│   ├── MultiBarHP.cs         # 多层血条
│   ├── Billboard.cs          # 血条始终面向摄像机
│   └── PlayerHUD.cs          # 玩家 HUD
├── Scenes/
│   └── CityNew.unity         # 主游戏场景
├── Resources/                # 运行时加载资源
│   ├── Prefabs/              # SPIDER 敌人预制体
│   ├── Effects/              # 子弹、枪火、命中特效
│   └── Voice/                # 音效（枪声、爆炸、脚步声）
├── Infima Games/             # Low Poly Shooter Pack（武器/角色/动画）
├── LowPolyBuildings/         # 城市场景建筑
└── model/                    # 蜘蛛机器人模型
```

## 核心系统

### 敌人 AI
两状态有限状态机：**Navigate** → **Attack**

- 防御塔始终可见，刷出即冲向水塔
- 玩家进入 30m 范围且比塔更近时切换为目标
- NavMesh 寻路，0.3s 路径更新间隔

### 波次系统
- 水塔周围 20-40m 环状区域随机 NavMesh 刷怪
- 难度递增：每波 +2 敌人、+15% HP、+5% 移速
- 对象池复用，减少 GC 开销

### 武器系统
- 左键射击，右键瞄准
- 后坐力上跳 + 回弹动画
- 子弹命中敌人 20 伤害，命中可破坏物可打飞

## 依赖

- Unity 2022.3+
- TextMesh Pro
- AI Navigation (NavMesh)

## 第三方资源

- [Low Poly Shooter Pack](https://assetstore.unity.com/packages/templates/packs/low-poly-shooter-pack-free-sample-243934) — 武器、角色、动画
- LowPolyBuildings — 城市场景
- SCI FI ROBOTS — 蜘蛛机器人
