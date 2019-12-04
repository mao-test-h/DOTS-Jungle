# DOTS-Jungle

DOTS + MonoBehaviour連携を踏まえたゲームサンプル  





### WebGL版について

WebGL版をunityroomで公開中  
→ [ウッホホウッホ](https://unityroom.com/games/uhohohouhho)

![jungle6](https://user-images.githubusercontent.com/17098415/69494705-2565c800-0f02-11ea-8fd1-b25de3fee158.gif)

※Assetの再配布制限やWebGL版の制約に伴い、以下の差分有り。

- JobSystemでの並列処理及びBurstCompilerが使えないので全体的にパフォーマンスは悪い
    - 前者の並列化は正確に言うとオプションで有効化可能らしいが...今回は有効にしておらず
    - パフォーマンスを考慮してステージ面積や敵の数を減らしてある
- こちらは「ゴリラがジャングルでバナナを探して集めるゲーム」と言う仕様に沿って各種Asset(モデル/BGMなど)をフル活用
    - これらは再配布不可能なので、こちらのソース公開版は一部モデルやBGMをプリミティブやフリーのものに差し替え
    




## Supported Unity versions

Unity 2019.3.0f1+

### DOTS Packages

- Entities preview.4 - 0.3.0  
- Hybrid Renderer preview.4 - 0.3.0  



# License

- [neuecc/UniRx](https://github.com/neuecc/UniRx/blob/master/LICENSE)
- [Cysharp/UniTask](https://github.com/Cysharp/UniTask/blob/master/LICENSE)
- [keijiro/KinoGlitch](https://github.com/keijiro/KinoGlitch)
- [mao-test-h/URPGlitch](https://github.com/mao-test-h/URPGlitch/blob/master/LICENSE)


## BGM/SE

- [魔王魂](https://maoudamashii.jokersounds.com/)
