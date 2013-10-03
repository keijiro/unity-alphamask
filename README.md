Unity でマスクを使う
====================

現在、モバイルプラットフォームにおいてアルファ付きのテクスチャ画像を扱うことは、比較的困難なハードルとなっています。次のような問題点があるためです。

- iOS の場合：アルファ付きの PVRTC は非常に画質が低い。
- Android の場合：ETC ではそもそもアルファが使えない。

いくつかの改善方法が考えられる（[例１](https://github.com/keijiro/unity-dither4444)、[例２](https://github.com/keijiro/unity-pvr-cleaner)）ものの、プラットフォームによって異なる手法を用いることは、煩雑さを生み出す要因となりかねません。

ここでは、この問題を解決する手法として、Unity でマスクを扱う方法について解説します。マスクを用いることによりアルファチャンネルが不要になるため、上記の問題を一括して解決できる可能性があります。

マスクの設定
------------

下図のように、色成分を含むベース画像と、アルファのみを含むマスク画像を、個別のテクスチャに格納します。

![Textures](https://github.com/keijiro/unity-alphamask/raw/gh-pages/Textures.png)

このとき、マスクはグレイスケールで不透明度を表すようにします。

そして、それぞれのテクスチャをアルファ無しの圧縮形式に変換します。具体的には、iOS であれば PVRTC RGB 4bit を、Android であれば ETC RGB 4 bit を用います。

描画時にはマスクの設定ができる特殊なシェーダーを用います。

![Inspector](https://github.com/keijiro/unity-alphamask/raw/gh-pages/Inspector.png)

これにより、1 ピクセルあたり 8 bit のコストで、比較的良好な画質での描画が実現できます。

エディタスクリプトの実装例
--------------------------

この手法は、ベースとマスクを別々のファイルに用意し、手動でインポートしていくことによっても実現が可能です。

エディタスクリプトを使えば自動化も可能です。ここでは例として、png ファイルからアルファチャンネルを分離して自動的にマスクを生成するスクリプトを組んでみました。

[Editor/TextureModifier.cs](https://github.com/keijiro/unity-alphamask/blob/master/Assets/Editor/TextureModifier.cs)

このスクリプトでは、ファイル名の末尾が “with alpha.png” の場合に、そのファイルからアルファチャンネルを抜き出してマスク用テクスチャを生成します。そして各々のテクスチャを適切な圧縮フォーマットで再変換します。

このようなスクリプトによりある程度の自動化が可能ですが、以下のようなデメリットもあります。

- プラットフォーム切り替え時に再インポート (Reimport) を手動で発動しなければならない。
- プラットフォーム切り替え時にアセットが更新される（バージョン管理に影響を与える可能性がある）。

ワークフローによっては、手動でも十分な作業効率を得られる場合があります。ケースバイケースで自動化の検討を行うのが望ましいです。

シェーダーの実装例
------------------

Unity の標準シェーダーには「マスクの設定が可能なシェーダー」が存在しないため、これを自前で実装する必要があります。以下はその一例です。

[SpriteWithMask.shader](https://github.com/keijiro/unity-alphamask/blob/master/Assets/SpriteWithMask.shader)

このシェーダーではマスクの他に色 (Color) が設定可能になっています。(127,127,127,127) で元の色がそのまま出力され、数値の増減により明暗両方への調整が可能です。

テスト結果
----------

#### iOS の場合

左はベース、右はマスク。それぞれ PVRTC RGB 4bit で圧縮済みです。

![iOS Base Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/PVRTC.png)![iOS Mask Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/PVRTC Mask.png)

合成して表示すると以下のようになりました。

![iOS Result](https://github.com/keijiro/unity-alphamask/raw/gh-pages/PVRTC Result.png)

#### Android の場合

左はベース、右はマスク。それぞれ ETC RGB 4bit で圧縮済みです。

![Android Base Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/ETC.png)![Android Mask Texture](https://github.com/keijiro/unity-alphamask/raw/gh-pages/ETC Mask.png)

合成して表示すると以下のようになりました。

![Android Result](https://github.com/keijiro/unity-alphamask/raw/gh-pages/ETC Result.png)

謝辞（テスト画像について）
--------------------------

上でテストに用いている画像 (Test A, Test B) は[テラシュールウェア](http://terasur.blog.fc2.com)さんよりご提供いただいたものです。 **この画像を検証以外に用いることは避けてください。**

スクリプトの使用について
------------------------

このプロジェクトに含まれるスクリプト (TextureModifier.cs) は商用・非商用を問わず自由に利用して構いません。
