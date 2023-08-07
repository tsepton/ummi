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

### Further instructions
- Download [a sentence transformer model](https://huggingface.co/sentence-transformers/all-MiniLM-L6-v2/blob/main/README.md) from huggingface (only all-MiniLM-L6-v2 has been tested for now). 
  - You will need to [download the entire ONNX folder](https://huggingface.co/docs/transformers/serialization), and place it inside the `StreamingAssets` folder of your project (`Assets/StreamingAssets/<onnx-folder>`). 
  - Either update the path specified to the UmmiSTC Monobehaviour, or make sure the folder is named `all-MiniLM-L6-v2` and contains `model.onnx` and `vocab.txt` files.
- Download [a Whisper model](https://huggingface.co/ggerganov/whisper.cpp) and place it inside `StreamingAssets/Whisper`. 
  - Again, make sure the path provided to the WhisperManager Monobehaviour matches your model path.  

### Example scene
Check [this repository](https://github.com/tsepton/ummi) for an example scene. 

## Acknowledgements
Other dependencies used are 

- [Whisper.unity](https://github.com/Macoron/whisper.unity)
 (MIT Licensed), 
- [BertTokenizers](https://github.com/NMZivkovic/BertTokenizers) (MIT Licensed).

However, these were cloned inside the repo and therefore, there is no need to import them. 

We also thank [Sentence Transformers: Multilingual Sentence, Paragraph, and Image Embeddings using BERT & Co.](https://github.com/UKPLab/sentence-transformers) for their work which inspired this project. 

## License
This project is licensed under the MIT License.
