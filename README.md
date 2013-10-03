マスク画像の分離による圧縮テクスチャ画質の改善
==============================================

動機
----

現状、モバイルプラットフォームにおいてアルファ付きのテクスチャ画像を扱うことは、比較的困難な課題となっています。次のような問題点があるためです。

- iOS の場合：アルファ付きの PVRTC は非常に画質が低い。
- Android の場合：ETC ではそもそもアルファが使えない。

いくつかの改善方法が考えられる（[例１](https://github.com/keijiro/unity-dither4444)、[例２](https://github.com/keijiro/unity-pvr-cleaner)）ものの、プラットフォームによって最適な方法が異なるため、マルチプラットフォーム同時開発を行うにあたっては、煩雑さを生み出す要因となりかねません。

ここでは、iOS/Android 両プラットフォームにおいて有効な手法として、マスク画像の分離による圧縮テクスチャ画質の改善手法を説明します。

手法の解説
----------

下図のように、色成分を含む画像と、アルファ（マスク）情報を含む画像を、別々のテクスチャへ格納します。

![Textures](https://github.com/keijiro/unity-alphamask/raw/gh-pages/Textures.png)

なおこのとき、マスク画像はアルファチャンネルを使用せず、グレイスケールで不透明率を表すようにします。

そして、それぞれの画像をアルファ無しの圧縮形式で変換します。具体的には、iOS の場合であれば PVRTC RGB 4bit、Android の場合は ETC RGB 4 bit を用います。

描画時にはマスクの設定ができる特殊なシェーダーを用います。

![Inspector](https://github.com/keijiro/unity-alphamask/raw/gh-pages/Inspector.png)

これにより、1 ピクセルあたり 8 bit のコストで、比較的良好な画質での描画が実現できます。

エディタスクリプトの実装例
--------------------------

この手法は、ベース画像とマスク画像を別々のファイルに用意し、手動で設定していくことによっても実現が可能です。

エディタスクリプトを使えば自動化も可能です。ここでは例として、png ファイルからアルファチャンネルを分離して自動的にマスク用テクスチャを生成するスクリプトを組んでみました。

[Editor/TextureModifier.cs](https://github.com/keijiro/unity-alphamask/blob/master/Assets/Editor/TextureModifier.cs)

このスクリプトでは、ファイル名の末尾が “with alpha.png” の場合に、そのファイルからアルファチャンネルを抜き出してマスク用テクスチャを生成します。そして各々のテクスチャを適切な圧縮フォーマットに再変換します。

このようなスクリプトによりある程度の自動化が可能ですが、再インポートを手動で発行しなければいけない、プラットフォーム切り替え時にアセットが更新されバージョン管理に影響を与える可能性がある、等々のデメリットがあります。

ワークフローによっては、手動設定でも十分な作業効率が得られる場合もあります。ケースバイケースで自動化の検討を行うのが望ましいと思われます。

テスト結果
----------

#### iOS の場合

左はベース画像、右はマスク画像。それぞれ PVRTC RGB 4bit で圧縮済みです。

![iOS Base Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/PVRTC.png)![iOS Mask Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/PVRTC Mask.png)

合成して表示すると以下のようになりました。

![iOS Result](https://github.com/keijiro/unity-alphamask/raw/gh-pages/PVRTC Result.png)

#### Android の場合

左はベース画像、右はマスク画像。それぞれ ETC RGB 4bit で圧縮済みです。

![Android Base Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/ETC.png)![Android Mask Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/ETC Mask.png)

合成して表示すると以下のようになりました。

![Android Result](https://github.com/keijiro/unity-alphamask/raw/gh-pages/ETC Result.png)

謝辞（テスト画像について）
--------------------------

上でテストに用いている画像 (Test A, Test B) は[テラシュールウェア](http://terasur.blog.fc2.com)さんよりご提供いただいたものです。 **この画像を検証以外に用いることは避けてください。**

スクリプトの使用について
------------------------

このプロジェクトに含まれるスクリプト (TextureModifier.cs) は商用・非商用を問わず自由に利用して構いません。
