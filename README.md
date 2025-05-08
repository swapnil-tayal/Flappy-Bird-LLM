
https://github.com/user-attachments/assets/339aa4b1-43ce-4662-a72e-7a5c43298292
# Flappy-Bird-LLM

```
default_settings: null
behaviors:
  My Behavior:
    trainer_type: ppo
    hyperparameters:
      batch_size: 1024
      buffer_size: 10240
      learning_rate: 0.0003
      beta: 0.005
      epsilon: 0.2
      lambd: 0.95
      num_epoch: 3
      learning_rate_schedule: linear
    network_settings:
      normalize: false
      hidden_units: 128
      num_layers: 2
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
    keep_checkpoints: 5
    max_steps: 2600000
    time_horizon: 64
    summary_freq: 50000
```
![Screenshot 2025-05-08 172216](https://github.com/user-attachments/assets/2bf7c873-5563-4ed0-9eaa-83773252a15f)


https://github.com/user-attachments/assets/cadfd95a-4623-4554-8319-89e440e69d48


