import json
import os


class FicheiroEmpresas:
    def __init__(self, empresas=None):
        self.empresas = empresas if empresas is not None else []


class Dados:
    ficheiro_empresas = None
    empresas = None
    sites = []
    FICHEIRO_EMPRESAS = os.path.abspath(
        os.path.join(os.path.dirname(__file__), ".env/empresas.json")
    )
    FICHEIRO_SITES = os.path.abspath(
        os.path.join(os.path.dirname(__file__), ".env/sites.json")
    )

    @staticmethod
    def load():
        if os.path.exists(Dados.FICHEIRO_EMPRESAS):
            with open(Dados.FICHEIRO_EMPRESAS, "r", encoding="utf-8") as f:
                empresas_list = json.load(f)
            Dados.ficheiro_empresas = FicheiroEmpresas(
                [Empresa.from_dict(e) for e in empresas_list]
            )
            Dados.empresas = Dados.ficheiro_empresas.empresas

        else:
            Dados.ficheiro_empresas = FicheiroEmpresas()
            Dados.empresas = Dados.ficheiro_empresas.empresas
            # Dados.save_empresas()
        if os.path.exists(Dados.FICHEIRO_SITES):
            with open(Dados.FICHEIRO_SITES, "r", encoding="utf-8") as f:
                s = json.load(f)
            for a in s:
                if len(a) > 1:
                    Dados.sites.append((a[0], a[1]))
                else:
                    Dados.sites.append((a[0], None))

    @staticmethod
    def desencriptar_passwords(master_password):
        for empresa in Dados.empresas:
            pass  # Implement decryption if needed

    @staticmethod
    def save_empresas():
        with open(Dados.FICHEIRO_EMPRESAS, "w", encoding="utf-8") as f:
            json.dump(
                [e.__dict__ for e in Dados.ficheiro_empresas.empresas],
                f,
                ensure_ascii=False,
                indent=2,
            )

    @staticmethod
    def remove_empresa(empresa_right_clicked):
        Dados.empresas.remove(empresa_right_clicked)
        Dados.save_empresas()


class Empresa:
    def __init__(self, nome, codigo, nif):
        self.nome = nome
        self.codigo = codigo
        self.nif = nif
        self.password_at = None
        self.password_at_encriptada = None
        self.niss = None
        self.password_ss = None
        self.password_ss_encriptada = None
        self.nome_do_responsavel = None
        self.telefone_do_responsavel = None
        self.email_do_responsavel = None
        self.codigo_certidao_permanente = None

    def __str__(self):
        return f"{self.nif} {self.nome}"

    def clone(self):
        import copy

        return copy.deepcopy(self)

    @staticmethod
    def from_dict(data):
        empresa = Empresa(
            data.get("nome", data.get("Nome", "")),
            data.get("codigo", data.get("Codigo", "")),
            data.get("nif", data.get("NIF", "")),
        )
        empresa.password_at = data.get("password_at", data.get("PasswordAT"))
        empresa.password_at_encriptada = data.get(
            "password_at_encriptada", data.get("PasswordATEncriptada")
        )
        empresa.niss = data.get("niss", data.get("NISS"))
        empresa.password_ss = data.get("password_ss", data.get("PasswordSS"))
        empresa.password_ss_encriptada = data.get(
            "password_ss_encriptada", data.get("PasswordSSEncriptada")
        )
        empresa.nome_do_responsavel = data.get(
            "nome_do_responsavel", data.get("NomeDoResponsavel")
        )
        empresa.telefone_do_responsavel = data.get(
            "telefone_do_responsavel", data.get("TelefoneDoResponsavel")
        )
        empresa.email_do_responsavel = data.get(
            "email_do_responsavel", data.get("EmailDoResponsavel")
        )
        empresa.codigo_certidao_permanente = data.get(
            "codigo_certidao_permanente", data.get("CodigoCertidaoPermanente")
        )
        return empresa
