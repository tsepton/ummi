# Unity Multimodal Interface
> Ummi for short, pronounced yummy.
<p align="center">
  <img src="./icon.png" alt="alt text" width="200"/>
</p>

## Requirements
This project uses .NET packages  
```bash
$ dotnet add package Microsoft.ML
$ dotnet add package Microsoft.ML.OnnxRuntime
$ dotnet add package Microsoft.ML.OnnxTransformer
```
If you init a new Unity project, simply use [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) to install these dependencies. 

## Acknowledgements
Other dependencies used are 

- [Whisper.unity](https://github.com/Macoron/whisper.unity)
 (MIT Licensed), 
- [BertTokenizers](https://github.com/NMZivkovic/BertTokenizers) (MIT Licensed).

However, these were cloned inside the repo and therefore, there is no need to import them. 

We also thank [Sentence Transformers: Multilingual Sentence, Paragraph, and Image Embeddings using BERT & Co.](https://github.com/UKPLab/sentence-transformers) for their work which inspired this project. 

## License
This project is licensed under the MIT License.
